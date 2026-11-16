using Microsoft.EntityFrameworkCore;
using Portfolio.Application.Common;
using Portfolio.Application.Common.Exceptions;
using Portfolio.Application.Common.Interfaces;
using Portfolio.Application.Figures;
using Portfolio.Application.Posts.Dtos;
using Portfolio.Domain.Entities;
using Portfolio.Domain.Enums;

namespace Portfolio.Application.Posts;

/// <summary>
/// Default <see cref="IPostService"/>. Public methods only ever read published posts;
/// admin methods see drafts too. Reading time is recalculated on every save.
/// </summary>
public sealed class PostService(IAppDbContext db, ICurrentUser currentUser, TimeProvider clock, ISiteEvents events) : IPostService
{
    /// <inheritdoc />
    public async Task<IReadOnlyList<PostSummaryResponse>> GetPublishedAsync(CancellationToken ct)
    {
        var posts = await db.Posts.AsNoTracking()
            .Where(p => p.Status == PostStatus.Published)
            .OrderByDescending(p => p.PublishedOn)
            .ToListAsync(ct);
        return posts.Select(p => p.ToSummary()).ToList();
    }

    /// <inheritdoc />
    public async Task<PostDetailResponse?> GetPublishedBySlugAsync(string slug, CancellationToken ct)
    {
        var post = await db.Posts.AsNoTracking()
            .FirstOrDefaultAsync(p => p.Slug == slug && p.Status == PostStatus.Published, ct);
        return post?.ToDetail();
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<AdminPostResponse>> GetAllForAdminAsync(CancellationToken ct)
    {
        var posts = await db.Posts.AsNoTracking().OrderByDescending(p => p.UpdatedAt).ToListAsync(ct);
        return posts.Select(p => p.ToAdmin()).ToList();
    }

    /// <inheritdoc />
    public async Task<AdminPostResponse> GetByIdAsync(Guid id, CancellationToken ct) =>
        (await FindAsync(id, ct)).ToAdmin();

    /// <inheritdoc />
    public async Task<AdminPostResponse?> GetBySlugForAdminAsync(string slug, CancellationToken ct)
    {
        var post = await db.Posts.AsNoTracking().FirstOrDefaultAsync(p => p.Slug == slug, ct);
        return post?.ToAdmin();
    }

    /// <inheritdoc />
    public async Task<AdminPostResponse> CreateDraftAsync(SavePostRequest request, CancellationToken ct)
    {
        var slug = string.IsNullOrWhiteSpace(request.Slug) ? SlugRules.FromTitle(request.Title) : request.Slug;
        if (!SlugRules.IsValid(slug))
            throw new RequestValidationException(nameof(request.Slug), "Could not build a valid slug from the title; please set one.");
        await EnsureSlugFreeAsync(slug, null, ct);

        var post = new Post { Id = Guid.NewGuid(), Slug = slug, Status = PostStatus.Draft, AuthorId = currentUser.UserId };
        Apply(post, request);
        db.Posts.Add(post);
        await db.SaveChangesAsync(ct);
        return post.ToAdmin();
    }

    /// <inheritdoc />
    public async Task<AdminPostResponse> UpdateAsync(Guid id, SavePostRequest request, CancellationToken ct)
    {
        var post = await FindAsync(id, ct);
        if (!string.IsNullOrWhiteSpace(request.Slug) && request.Slug != post.Slug)
        {
            await EnsureSlugFreeAsync(request.Slug, id, ct);
            post.Slug = request.Slug;
        }
        Apply(post, request);
        await db.SaveChangesAsync(ct);
        // Drafts are not public, so only an edit to a published post changes the site.
        if (post.Status == PostStatus.Published) await events.ContentChangedAsync(SiteTopics.Posts, ct);
        return post.ToAdmin();
    }

    /// <inheritdoc />
    public async Task<AdminPostResponse> PublishAsync(Guid id, CancellationToken ct)
    {
        var post = await FindAsync(id, ct);
        post.Status = PostStatus.Published;
        post.PublishedOn ??= DateOnly.FromDateTime(clock.GetUtcNow().UtcDateTime);
        await db.SaveChangesAsync(ct);
        await events.ContentChangedAsync(SiteTopics.Posts, ct);
        return post.ToAdmin();
    }

    /// <inheritdoc />
    public async Task<AdminPostResponse> UnpublishAsync(Guid id, CancellationToken ct)
    {
        var post = await FindAsync(id, ct);
        post.Status = PostStatus.Draft;
        await db.SaveChangesAsync(ct);
        await events.ContentChangedAsync(SiteTopics.Posts, ct);
        return post.ToAdmin();
    }

    /// <inheritdoc />
    public async Task DeleteAsync(Guid id, CancellationToken ct)
    {
        var post = await FindAsync(id, ct);
        var wasPublic = post.Status == PostStatus.Published;
        db.Posts.Remove(post);
        await db.SaveChangesAsync(ct);
        if (wasPublic) await events.ContentChangedAsync(SiteTopics.Posts, ct);
    }

    private async Task<Post> FindAsync(Guid id, CancellationToken ct) =>
        await db.Posts.FirstOrDefaultAsync(p => p.Id == id, ct) ?? throw new NotFoundException("Post", id);

    private async Task EnsureSlugFreeAsync(string slug, Guid? exceptId, CancellationToken ct)
    {
        if (await db.Posts.AnyAsync(p => p.Slug == slug && p.Id != exceptId, ct))
            throw new ConflictException($"Another post already uses the slug '{slug}'.");
    }

    /// <summary>Copies editable fields from the request and runs the business rules.</summary>
    private static void Apply(Post post, SavePostRequest r)
    {
        if (r.LeadFigure is not null && !FigureCatalog.Exists(r.LeadFigure))
            throw new RequestValidationException(nameof(r.LeadFigure), $"Unknown figure '{r.LeadFigure}'. See GET /api/figures.");
        if (r.CoverUrl is not null && string.IsNullOrWhiteSpace(r.CoverAlt))
            throw new RequestValidationException(nameof(r.CoverAlt), "A cover image needs alt text.");

        post.Title = r.Title.Trim();
        post.Excerpt = r.Excerpt.Trim();
        post.Body = r.Body;
        post.Category = r.Category.Trim();
        post.Layer = r.Layer;
        post.Tags = r.Tags.Select(t => t.Trim().ToLowerInvariant()).Where(t => t.Length > 0).Distinct().ToList();
        post.LeadFigure = string.IsNullOrWhiteSpace(r.LeadFigure) ? null : r.LeadFigure;
        post.CoverUrl = r.CoverUrl;
        post.CoverAlt = r.CoverAlt;
        post.Featured = r.Featured;
        post.ReadingMinutes = ReadingTime.Estimate(r.Body);
    }
}
