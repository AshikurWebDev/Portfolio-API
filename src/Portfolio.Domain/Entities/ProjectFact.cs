namespace Portfolio.Domain.Entities;

/// <summary>A label/value row on a project card.</summary>
public class ProjectFact
{
    /// <summary>Row label, e.g. "Users".</summary>
    public string Label { get; set; } = string.Empty;

    /// <summary>Row value, e.g. "~110 students, 25 staff".</summary>
    public string Value { get; set; } = string.Empty;
}
