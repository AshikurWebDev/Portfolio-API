namespace Portfolio.Application.Skills.Dtos;

/// <summary>A Skills card with its skills in display order.</summary>
/// <param name="Id">Group id.</param>
/// <param name="Name">Card title.</param>
/// <param name="Skills">Skills in display order.</param>
public sealed record SkillGroupResponse(Guid Id, string Name, IReadOnlyList<SkillResponse> Skills);
