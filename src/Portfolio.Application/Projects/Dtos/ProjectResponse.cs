namespace Portfolio.Application.Projects.Dtos;

/// <summary>A project for the "Systems" section. Matches the website's <c>Project</c> type.</summary>
/// <param name="Id">Stable id.</param>
/// <param name="Title">Project name.</param>
/// <param name="Tags">Technology tags.</param>
/// <param name="Status">"live", "in-development" or "completed".</param>
/// <param name="StatusLabel">Status text for visitors.</param>
/// <param name="Facts">Extra spec rows.</param>
/// <param name="Summary">Short description.</param>
/// <param name="Highlights">Feature list.</param>
/// <param name="Media">Card media.</param>
/// <param name="Tone">"dark", "stone" or "accent".</param>
/// <param name="DemoUrl">Demo link or null.</param>
/// <param name="CodeUrl">Code link or null.</param>
/// <param name="CaseStudyUrl">Case study link or null.</param>
/// <param name="Order">Sort order.</param>
public sealed record ProjectResponse(
    string Id,
    string Title,
    IReadOnlyList<string> Tags,
    string Status,
    string StatusLabel,
    IReadOnlyList<ProjectFactResponse> Facts,
    string Summary,
    IReadOnlyList<ProjectHighlightResponse> Highlights,
    ProjectMediaResponse Media,
    string Tone,
    string? DemoUrl,
    string? CodeUrl,
    string? CaseStudyUrl,
    int Order);
