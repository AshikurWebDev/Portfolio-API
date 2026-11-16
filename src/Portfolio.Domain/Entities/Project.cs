using Portfolio.Domain.Common;
using Portfolio.Domain.Enums;

namespace Portfolio.Domain.Entities;

/// <summary>
/// A project shown in the "Systems" section. Lists such as facts and highlights
/// are stored as JSON columns (EF Core owned types mapped with ToJson).
/// </summary>
public class Project : AuditableEntity
{
    /// <summary>Stable id used by the website, e.g. "coaching-erp".</summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>Project name.</summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>Technology tags.</summary>
    public List<string> Tags { get; set; } = [];

    /// <summary>Honest status.</summary>
    public ProjectStatus Status { get; set; }

    /// <summary>Status text shown to visitors, e.g. "In daily use".</summary>
    public string StatusLabel { get; set; } = string.Empty;

    /// <summary>Extra spec rows, e.g. Users: ~110 students.</summary>
    public List<ProjectFact> Facts { get; set; } = [];

    /// <summary>Short description.</summary>
    public string Summary { get; set; } = string.Empty;

    /// <summary>Feature list; unfinished items are flagged.</summary>
    public List<ProjectHighlight> Highlights { get; set; } = [];

    /// <summary>What the project card shows (image, video or live fragment).</summary>
    public ProjectMedia Media { get; set; } = new();

    /// <summary>Frame colour behind the media: "dark", "stone" or "accent".</summary>
    public string Tone { get; set; } = "dark";

    /// <summary>Public demo link, or null while not public.</summary>
    public string? DemoUrl { get; set; }

    /// <summary>Source code link, or null while not public.</summary>
    public string? CodeUrl { get; set; }

    /// <summary>Case study link, or null.</summary>
    public string? CaseStudyUrl { get; set; }

    /// <summary>Sort order; lower first.</summary>
    public int SortOrder { get; set; }
}
