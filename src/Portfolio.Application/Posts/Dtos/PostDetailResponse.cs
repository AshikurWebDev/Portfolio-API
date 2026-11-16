using Portfolio.Domain.Enums;

namespace Portfolio.Application.Posts.Dtos;

/// <summary>A published post with its Markdown body. Matches the website's <c>PostDetail</c> type.</summary>
/// <param name="Slug">URL part.</param>
/// <param name="Title">Headline.</param>
/// <param name="Excerpt">Short summary.</param>
/// <param name="PublishedAt">Publication date, "yyyy-MM-dd".</param>
/// <param name="ReadingMinutes">Estimated reading time.</param>
/// <param name="Category">Category label.</param>
/// <param name="Layer">System layer.</param>
/// <param name="Tags">Tags.</param>
/// <param name="LeadFigure">Figure id for the post header, if any.</param>
/// <param name="Cover">Cover image, if any.</param>
/// <param name="Featured">Preferred on the home page.</param>
/// <param name="Body">Markdown body with embed lines.</param>
public sealed record PostDetailResponse(
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
    bool Featured,
    string Body);
