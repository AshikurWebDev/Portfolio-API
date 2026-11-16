using System.ComponentModel.DataAnnotations;

namespace Portfolio.Application.Skills.Dtos;

/// <summary>Create or rename a Skills card.</summary>
public sealed record SaveSkillGroupRequest
{
    /// <summary>Card title, e.g. "Databases".</summary>
    [Required, MaxLength(60)]
    public required string Name { get; init; }
}
