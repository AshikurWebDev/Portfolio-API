using Portfolio.Domain.Enums;

namespace Portfolio.Application.Posts.Dtos;

/// <summary>A post as the admin (and the MCP server) sees it: drafts included, with ids and timestamps.</summary>
/// <param name="Id">Post id.</param>
/// <param name="Slug">URL part.</param>
/// <param name="Title">Headline.</param>
/// <param name="Excerpt">Short summary.</param>
/// <param name="Body">Markdown body.</param>
/// <param name="Category">Category label.</param>
/// <param name="Layer">System layer.</param>
/// <param name="Tags">Tags.</param>
/// <param name="LeadFigure">Figure id for the header.</param>
/// <param name="CoverUrl">Cover image URL.</param>
/// <param name="CoverAlt">Cover alt text.</param>
/// <param name="Featured">Preferred on the home page.</param>
/// <param name="Status">Draft or published.</param>
/// <param name="PublishedOn">Publication date, null for a never-published draft.</param>
/// <param name="ReadingMinutes">Estimated reading time.</param>
/// <param name="CreatedAt">Created (UTC).</param>
/// <param name="UpdatedAt">Last changed (UTC).</param>
public sealed record AdminPostResponse(
    Guid Id,
    string Slug,
    string Title,
    string Excerpt,
    string Body,
    string Category,
    SystemLayer Layer,
    IReadOnlyList<string> Tags,
    string? LeadFigure,
    string? CoverUrl,
    string? CoverAlt,
    bool Featured,
    PostStatus Status,
    DateOnly? PublishedOn,
    int ReadingMinutes,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);
