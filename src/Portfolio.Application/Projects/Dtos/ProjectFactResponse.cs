namespace Portfolio.Application.Projects.Dtos;

/// <summary>A label/value row on a project card.</summary>
/// <param name="Label">Row label.</param>
/// <param name="Value">Row value.</param>
public sealed record ProjectFactResponse(string Label, string Value);
