using Microsoft.AspNetCore.Mvc;
using Portfolio.Application.Skills;
using Portfolio.Application.Skills.Dtos;

namespace Portfolio.Api.Controllers;

/// <summary>Public, read-only Skills section. Managed in the admin dashboard.</summary>
[ApiController]
[Route("api/skills")]
[Produces("application/json")]
public sealed class SkillsController(ISkillService skills) : ControllerBase
{
    /// <summary>Lists the Skills cards with their skills, in display order.</summary>
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<SkillGroupResponse>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<SkillGroupResponse>>> GetAll(CancellationToken ct) =>
        Ok(await skills.GetAllAsync(ct));
}
