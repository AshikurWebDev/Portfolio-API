using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Portfolio.Application.Common.Exceptions;
using Portfolio.Application.Common.Images;
using Portfolio.Application.Common.Interfaces;
using Portfolio.Application.Design.Dtos;
using Portfolio.Domain.Entities;

namespace Portfolio.Application.Design;

/// <summary>
/// Default <see cref="IDesignPostService"/>. Talks to the database through <see cref="IAppDbContext"/>
/// and compresses uploads with <see cref="IImageCompressor"/>.
/// <para>
/// Performance note: <c>ImageData</c> (up to 100 KB per row) is only read by <see cref="GetImageAsync"/>.
/// Every list query uses <c>Select</c> into a DTO, so EF Core never loads the bytes for lists.
/// </para>
/// </summary>
public sealed class DesignPostService(IAppDbContext db, IImageCompressor compressor, ISiteEvents events) : IDesignPostService
{
    /// <inheritdoc />
    public async Task<IReadOnlyList<DesignPostResponse>> GetVisibleAsync(CancellationToken ct) =>
        await db.DesignPosts
            .AsNoTracking()
            .Where(d => d.IsVisible)
            .OrderBy(d => d.SortOrder).ThenBy(d => d.CreatedAt)
            .Select(d => new DesignPostResponse(
                d.Id,
                "/api/design-posts/" + d.Id + "/image?v=" + d.ImageVersion,
                d.Caption,
                d.Link,
                d.CreatedAt,
                d.ImageWidth,
                d.ImageHeight))
            .ToListAsync(ct);

    /// <inheritdoc />
    public async Task<DesignImageFile?> GetImageAsync(Guid id, CancellationToken ct) =>
        await db.DesignPosts
            .AsNoTracking()
            .Where(d => d.Id == id)
            .Select(d => new DesignImageFile(d.ImageData, d.ImageVersion))
            .FirstOrDefaultAsync(ct);

    /// <inheritdoc />
    public async Task<IReadOnlyList<AdminDesignPostResponse>> GetAllAsync(CancellationToken ct) =>
        await ToAdminResponse(db.DesignPosts).ToListAsync(ct);

    /// <inheritdoc />
    public async Task<AdminDesignPostResponse> GetAsync(Guid id, CancellationToken ct) =>
        // Filter on the entity first, then project: EF Core cannot translate a Where on a DTO constructor.
        await ToAdminResponse(db.DesignPosts.Where(d => d.Id == id)).FirstOrDefaultAsync(ct)
            ?? throw new NotFoundException("Design item", id);

    /// <inheritdoc />
    public async Task<AdminDesignPostResponse> CreateAsync(Stream image, string caption, string? link, CancellationToken ct)
    {
        var (cleanCaption, cleanLink) = Validate(caption, link);
        var compressed = compressor.CompressToWebp(image);

        // New items go to the end of the gallery.
        var nextOrder = await db.DesignPosts.Select(d => (int?)d.SortOrder).MaxAsync(ct) ?? -1;
        var entity = new DesignPost
        {
            Id = Guid.CreateVersion7(),
            Caption = cleanCaption,
            Link = cleanLink,
            SortOrder = nextOrder + 1,
            IsVisible = true,
        };
        SetImage(entity, compressed);

        db.DesignPosts.Add(entity);
        await db.SaveChangesAsync(ct);
        await events.ContentChangedAsync(SiteTopics.Design, ct);
        return await GetAsync(entity.Id, ct);
    }

    /// <inheritdoc />
    public async Task<AdminDesignPostResponse> UpdateAsync(Guid id, UpdateDesignPostRequest request, CancellationToken ct)
    {
        var entity = await FindAsync(id, ct);
        var (cleanCaption, cleanLink) = Validate(request.Caption, request.Link);
        entity.Caption = cleanCaption;
        entity.Link = cleanLink;
        entity.IsVisible = request.IsVisible;
        await db.SaveChangesAsync(ct);
        await events.ContentChangedAsync(SiteTopics.Design, ct);
        return await GetAsync(id, ct);
    }

    /// <inheritdoc />
    public async Task<AdminDesignPostResponse> ReplaceImageAsync(Guid id, Stream image, CancellationToken ct)
    {
        var entity = await FindAsync(id, ct);
        SetImage(entity, compressor.CompressToWebp(image));
        await db.SaveChangesAsync(ct);
        await events.ContentChangedAsync(SiteTopics.Design, ct);
        return await GetAsync(id, ct);
    }

    /// <inheritdoc />
    public async Task ReorderAsync(ReorderDesignPostsRequest request, CancellationToken ct)
    {
        // Loading tracked entities would also load every image's bytes, so we read only the ids
        // and send one small UPDATE per item with ExecuteUpdate (no bytes read).
        var existing = await db.DesignPosts.Select(d => d.Id).ToListAsync(ct);
        if (request.Ids.Count != existing.Count || request.Ids.Distinct().Count() != existing.Count || !request.Ids.All(existing.Contains))
            throw new RequestValidationException(nameof(request.Ids), "Send every design item id exactly once.");

        for (var i = 0; i < request.Ids.Count; i++)
        {
            var itemId = request.Ids[i];
            var order = i;
            await db.DesignPosts.Where(d => d.Id == itemId).ExecuteUpdateAsync(s => s.SetProperty(d => d.SortOrder, order), ct);
        }
        await events.ContentChangedAsync(SiteTopics.Design, ct);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(Guid id, CancellationToken ct)
    {
        var deleted = await db.DesignPosts.Where(d => d.Id == id).ExecuteDeleteAsync(ct);
        if (deleted == 0) throw new NotFoundException("Design item", id);
        await events.ContentChangedAsync(SiteTopics.Design, ct);
    }

    /// <summary>Admin shape, projected in SQL. Only the byte length is read, never the bytes.</summary>
    private static IQueryable<AdminDesignPostResponse> ToAdminResponse(IQueryable<DesignPost> source) =>
        source
            .AsNoTracking()
            .OrderBy(d => d.SortOrder).ThenBy(d => d.CreatedAt)
            .Select(d => new AdminDesignPostResponse(
                d.Id,
                d.Caption,
                d.Link,
                d.IsVisible,
                d.SortOrder,
                "/api/design-posts/" + d.Id + "/image?v=" + d.ImageVersion,
                d.ImageWidth,
                d.ImageHeight,
                d.ImageData.Length,
                d.CreatedAt,
                d.UpdatedAt));

    /// <summary>Tracked entity for an update (this one does load the bytes; fine for a single row).</summary>
    private async Task<DesignPost> FindAsync(Guid id, CancellationToken ct) =>
        await db.DesignPosts.FirstOrDefaultAsync(d => d.Id == id, ct)
            ?? throw new NotFoundException("Design item", id);

    private static void SetImage(DesignPost entity, CompressedImage image)
    {
        entity.ImageData = image.Data;
        entity.ImageWidth = image.Width;
        entity.ImageHeight = image.Height;
        // First 8 bytes of SHA-256 as hex: short, and changes whenever the picture changes.
        entity.ImageVersion = Convert.ToHexStringLower(SHA256.HashData(image.Data))[..16];
    }

    /// <summary>Trims input and checks the rules data annotations cannot express for multipart forms.</summary>
    private static (string Caption, string? Link) Validate(string caption, string? link)
    {
        var c = (caption ?? string.Empty).Trim();
        if (c.Length is 0 or > 200)
            throw new RequestValidationException("caption", "Write a caption of 1 to 200 characters.");

        var l = string.IsNullOrWhiteSpace(link) ? null : link.Trim();
        if (l is not null && (l.Length > 500 || !Uri.TryCreate(l, UriKind.Absolute, out var uri) || uri.Scheme is not ("http" or "https")))
            throw new RequestValidationException("link", "Use a full http(s) link, or leave it empty.");

        return (c, l);
    }
}
