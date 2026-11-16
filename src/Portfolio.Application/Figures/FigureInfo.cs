namespace Portfolio.Application.Figures;

/// <summary>A figure the website can render inside a post.</summary>
/// <param name="Id">Id used in ::figure{id=...} and in LeadFigure.</param>
/// <param name="Description">What the figure shows, to help writers (and AI agents) pick one.</param>
public sealed record FigureInfo(string Id, string Description);
