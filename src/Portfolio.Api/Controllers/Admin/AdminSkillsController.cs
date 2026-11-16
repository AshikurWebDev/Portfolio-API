using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Api.Extensions;
using Portfolio.Application.Skills;
using Portfolio.Application.Skills.Dtos;

namespace Portfolio.Api.Controllers.Admin;

/// <summary>Manage the Skills section: cards (groups) and the skills in them, with their percentages.</summary>
[ApiController]
[Route("api/admin/skills")]
[Produces("application/json")]
[Authorize(Policy = AuthenticationSetup.AdminPolicy)]
public sealed class AdminSkillsController(ISkillService skills) : ControllerBase
{
    /// <summary>All cards with their skills, in display order.</summary>
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<SkillGroupResponse>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<SkillGroupResponse>>> GetAll(CancellationToken ct) =>
        Ok(await skills.GetAllAsync(ct));

    /// <summary>Adds a card at the end.</summary>
    [HttpPost("groups")]
    [ProducesResponseType<SkillGroupResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<SkillGroupResponse>> CreateGroup(SaveSkillGroupRequest request, CancellationToken ct)
    {
        var group = await skills.CreateGroupAsync(request, ct);
        return CreatedAtAction(nameof(GetAll), null, group);
    }

    /// <summary>Renames a card.</summary>
    [HttpPut("groups/{id:guid}")]
    [ProducesResponseType<SkillGroupResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SkillGroupResponse>> RenameGroup(Guid id, SaveSkillGroupRequest request, CancellationToken ct) =>
        Ok(await skills.RenameGroupAsync(id, request, ct));

    /// <summary>Deletes a card and all of its skills.</summary>
    [HttpDelete("groups/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteGroup(Guid id, CancellationToken ct)
    {
        await skills.DeleteGroupAsync(id, ct);
        return NoContent();
    }

    /// <summary>Saves a new card order. Send every card id once, first card first.</summary>
    [HttpPut("groups/order")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ReorderGroups(ReorderRequest request, CancellationToken ct)
    {
        await skills.ReorderGroupsAsync(request, ct);
        return NoContent();
    }

    /// <summary>Adds a skill at the end of a card.</summary>
    [HttpPost("groups/{groupId:guid}/skills")]
    [ProducesResponseType<SkillResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SkillResponse>> CreateSkill(Guid groupId, SaveSkillRequest request, CancellationToken ct)
    {
        var skill = await skills.CreateSkillAsync(groupId, request, ct);
        return CreatedAtAction(nameof(GetAll), null, skill);
    }

    /// <summary>Saves a new skill order inside a card. Send every skill id of that card once.</summary>
    [HttpPut("groups/{groupId:guid}/skills/order")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ReorderSkills(Guid groupId, ReorderRequest request, CancellationToken ct)
    {
        await skills.ReorderSkillsAsync(groupId, request, ct);
        return NoContent();
    }

    /// <summary>Changes a skill (name, level, logo, note) or moves it to another card.</summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType<SkillResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SkillResponse>> UpdateSkill(Guid id, SaveSkillRequest request, CancellationToken ct) =>
        Ok(await skills.UpdateSkillAsync(id, request, ct));

    /// <summary>Deletes a skill.</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteSkill(Guid id, CancellationToken ct)
    {
        await skills.DeleteSkillAsync(id, ct);
        return NoContent();
    }
}
