using Portfolio.Application.Posts.Dtos;
using Portfolio.Domain.Entities;

namespace Portfolio.Application.Posts;

/// <summary>Maps <see cref="Post"/> entities to response DTOs. Kept in one place so every endpoint returns the same shape.</summary>
public static class PostMappings
{
    private static CoverResponse? Cover(Post p) =>
        p.CoverUrl is null ? null : new CoverResponse(p.CoverUrl, p.CoverAlt ?? string.Empty);

    /// <summary>Public list item. Only call for published posts (PublishedOn is set).</summary>
    public static PostSummaryResponse ToSummary(this Post p) =>
        new(p.Slug, p.Title, p.Excerpt, p.PublishedOn!.Value, p.ReadingMinutes, p.Category, p.Layer, p.Tags,
            p.LeadFigure, Cover(p), p.Featured);

    /// <summary>Public detail. Only call for published posts.</summary>
    public static PostDetailResponse ToDetail(this Post p) =>
        new(p.Slug, p.Title, p.Excerpt, p.PublishedOn!.Value, p.ReadingMinutes, p.Category, p.Layer, p.Tags,
            p.LeadFigure, Cover(p), p.Featured, p.Body);

    /// <summary>Admin view, drafts included.</summary>
    public static AdminPostResponse ToAdmin(this Post p) =>
        new(p.Id, p.Slug, p.Title, p.Excerpt, p.Body, p.Category, p.Layer, p.Tags, p.LeadFigure, p.CoverUrl,
            p.CoverAlt, p.Featured, p.Status, p.PublishedOn, p.ReadingMinutes, p.CreatedAt, p.UpdatedAt);
}
