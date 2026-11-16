using Portfolio.Domain.Common;

namespace Portfolio.Domain.Entities;

/// <summary>One skill with a self-rated level, shown as a bar in its <see cref="SkillGroup"/> card.</summary>
public class Skill : AuditableEntity
{
    /// <summary>Primary key.</summary>
    public Guid Id { get; set; }

    /// <summary>The card this skill belongs to.</summary>
    public Guid GroupId { get; set; }

    /// <summary>Navigation to the card.</summary>
    public SkillGroup? Group { get; set; }

    /// <summary>Skill name, e.g. "ASP.NET Core Web API".</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Self-rated level from 0 to 100 (percent).</summary>
    public int Level { get; set; }

    /// <summary>
    /// Key of the small logo shown next to the name, e.g. "react" or "postgresql".
    /// Only keys from <c>SkillIcons.Keys</c> are accepted; the website maps each key to its own SVG,
    /// so no markup is ever stored in the database.
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>Optional short note shown after the name, e.g. "normal knowledge".</summary>
    public string? Note { get; set; }

    /// <summary>Position inside the card; lower comes first.</summary>
    public int SortOrder { get; set; }
}
