namespace Portfolio.Application.Skills.Dtos;

/// <summary>One skill as the site and the admin see it.</summary>
/// <param name="Id">Skill id.</param>
/// <param name="Name">Skill name.</param>
/// <param name="Level">Self-rated level, 0 to 100.</param>
/// <param name="Icon">Logo key (see SkillIcons), or null for no logo.</param>
/// <param name="Note">Optional short note shown after the name.</param>
public sealed record SkillResponse(Guid Id, string Name, int Level, string? Icon, string? Note);
