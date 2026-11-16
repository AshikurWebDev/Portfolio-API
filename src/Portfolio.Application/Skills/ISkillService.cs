using Portfolio.Application.Skills.Dtos;

namespace Portfolio.Application.Skills;

/// <summary>
/// The Skills section: cards (groups) holding skills with a percentage.
/// Read by the public site; managed in the admin dashboard.
/// </summary>
public interface ISkillService
{
    /// <summary>All cards with their skills, in display order.</summary>
    Task<IReadOnlyList<SkillGroupResponse>> GetAllAsync(CancellationToken ct);

    /// <summary>Adds an empty card at the end.</summary>
    Task<SkillGroupResponse> CreateGroupAsync(SaveSkillGroupRequest request, CancellationToken ct);

    /// <summary>Renames a card.</summary>
    Task<SkillGroupResponse> RenameGroupAsync(Guid id, SaveSkillGroupRequest request, CancellationToken ct);

    /// <summary>Deletes a card and every skill in it.</summary>
    Task DeleteGroupAsync(Guid id, CancellationToken ct);

    /// <summary>Saves a new card order. The list must contain every card id exactly once.</summary>
    Task ReorderGroupsAsync(ReorderRequest request, CancellationToken ct);

    /// <summary>Adds a skill at the end of a card.</summary>
    Task<SkillResponse> CreateSkillAsync(Guid groupId, SaveSkillRequest request, CancellationToken ct);

    /// <summary>Changes a skill, and moves it to another card when <see cref="SaveSkillRequest.GroupId"/> differs.</summary>
    Task<SkillResponse> UpdateSkillAsync(Guid id, SaveSkillRequest request, CancellationToken ct);

    /// <summary>Deletes a skill.</summary>
    Task DeleteSkillAsync(Guid id, CancellationToken ct);

    /// <summary>Saves a new skill order inside one card. The list must contain every skill id of that card exactly once.</summary>
    Task ReorderSkillsAsync(Guid groupId, ReorderRequest request, CancellationToken ct);
}
