namespace Portfolio.Domain.Entities;

/// <summary>One feature of a project.</summary>
public class ProjectHighlight
{
    /// <summary>Feature text.</summary>
    public string Text { get; set; } = string.Empty;

    /// <summary>True while the feature is still being built.</summary>
    public bool InProgress { get; set; }
}
