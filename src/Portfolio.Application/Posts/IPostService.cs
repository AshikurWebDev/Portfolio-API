using Portfolio.Application.Posts.Dtos;

namespace Portfolio.Application.Posts;

/// <summary>Blog post use cases: public reading and admin management.</summary>
public interface IPostService
{
    /// <summary>All published posts, newest first. Drafts are never included.</summary>
    Task<IReadOnlyList<PostSummaryResponse>> GetPublishedAsync(CancellationToken ct);

    /// <summary>One published post by slug, or null when missing or still a draft.</summary>
    Task<PostDetailResponse?> GetPublishedBySlugAsync(string slug, CancellationToken ct);

    /// <summary>All posts for the admin, drafts included, most recently changed first.</summary>
    Task<IReadOnlyList<AdminPostResponse>> GetAllForAdminAsync(CancellationToken ct);

    /// <summary>One post by id for the admin. Throws <c>NotFoundException</c> when missing.</summary>
    Task<AdminPostResponse> GetByIdAsync(Guid id, CancellationToken ct);

    /// <summary>One post by slug for the admin (drafts included), or null.</summary>
    Task<AdminPostResponse?> GetBySlugForAdminAsync(string slug, CancellationToken ct);

    /// <summary>Creates a draft. Throws <c>ConflictException</c> when the slug is taken.</summary>
    Task<AdminPostResponse> CreateDraftAsync(SavePostRequest request, CancellationToken ct);

    /// <summary>Updates a post's content. Does not change its status.</summary>
    Task<AdminPostResponse> UpdateAsync(Guid id, SavePostRequest request, CancellationToken ct);

    /// <summary>Publishes a post. Sets the publication date the first time.</summary>
    Task<AdminPostResponse> PublishAsync(Guid id, CancellationToken ct);

    /// <summary>Turns a published post back into a draft (keeps its date).</summary>
    Task<AdminPostResponse> UnpublishAsync(Guid id, CancellationToken ct);

    /// <summary>Deletes a post permanently.</summary>
    Task DeleteAsync(Guid id, CancellationToken ct);
}
