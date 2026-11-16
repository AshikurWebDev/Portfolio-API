using System.ComponentModel.DataAnnotations;

namespace Portfolio.Application.Skills.Dtos;

/// <summary>Create or change a skill.</summary>
public sealed record SaveSkillRequest
{
    /// <summary>Skill name, e.g. "PostgreSQL".</summary>
    [Required, MaxLength(80)]
    public required string Name { get; init; }

    /// <summary>Self-rated level in percent.</summary>
    [Range(0, 100)]
    public int Level { get; init; }

    /// <summary>Logo key (see SkillIcons), or null/empty for no logo.</summary>
    [MaxLength(40)]
    public string? Icon { get; init; }

    /// <summary>Optional short note shown after the name.</summary>
    [MaxLength(60)]
    public string? Note { get; init; }

    /// <summary>When updating: move the skill to this card (it goes to the end). Ignored on create.</summary>
    public Guid? GroupId { get; init; }
}
