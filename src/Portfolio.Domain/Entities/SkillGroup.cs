using Portfolio.Domain.Common;

namespace Portfolio.Domain.Entities;

/// <summary>
/// A card in the Skills section, e.g. "Backend · .NET" or "Databases".
/// Holds the skills shown in it as bars with a percentage.
/// </summary>
public class SkillGroup : AuditableEntity
{
    /// <summary>Primary key.</summary>
    public Guid Id { get; set; }

    /// <summary>Card title shown on the site.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Position of the card; lower comes first.</summary>
    public int SortOrder { get; set; }

    /// <summary>The skills in this card. Deleting the group deletes them too.</summary>
    public List<Skill> Skills { get; set; } = [];
}
