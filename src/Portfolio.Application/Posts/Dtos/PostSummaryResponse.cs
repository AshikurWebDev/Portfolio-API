using Portfolio.Domain.Enums;

namespace Portfolio.Application.Posts.Dtos;

/// <summary>
/// A published post as listed on the blog (no body). Matches the website's
/// <c>Post</c> type in web/src/types/content.ts.
/// </summary>
/// <param name="Slug">URL part.</param>
/// <param name="Title">Headline.</param>
/// <param name="Excerpt">Short summary.</param>
/// <param name="PublishedAt">Publication date, "yyyy-MM-dd".</param>
/// <param name="ReadingMinutes">Estimated reading time.</param>
/// <param name="Category">Category label.</param>
/// <param name="Layer">System layer: ui, api, logic or data.</param>
/// <param name="Tags">Tags.</param>
/// <param name="LeadFigure">Figure id for the post header, if any.</param>
/// <param name="Cover">Cover image, if any.</param>
/// <param name="Featured">Preferred on the home page.</param>
public sealed record PostSummaryResponse(
    string Slug,
    string Title,
    string Excerpt,
    DateOnly PublishedAt,
    int ReadingMinutes,
    string Category,
    SystemLayer Layer,
    IReadOnlyList<string> Tags,
    string? LeadFigure,
    CoverResponse? Cover,
    bool Featured);
