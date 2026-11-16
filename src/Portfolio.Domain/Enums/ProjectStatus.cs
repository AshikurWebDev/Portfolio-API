namespace Portfolio.Domain.Enums;

/// <summary>Honest status of a portfolio project. Serialized as "live", "inDevelopment", "completed".</summary>
public enum ProjectStatus
{
    /// <summary>In real daily use.</summary>
    Live,

    /// <summary>Being built; no live users yet.</summary>
    InDevelopment,

    /// <summary>Finished and handed over.</summary>
    Completed,
}
