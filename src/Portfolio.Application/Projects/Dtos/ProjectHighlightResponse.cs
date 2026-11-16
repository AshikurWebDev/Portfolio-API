namespace Portfolio.Application.Projects.Dtos;

/// <summary>One feature of a project.</summary>
/// <param name="Text">Feature text.</param>
/// <param name="InProgress">True while still being built; omitted when false.</param>
public sealed record ProjectHighlightResponse(string Text, bool? InProgress);
