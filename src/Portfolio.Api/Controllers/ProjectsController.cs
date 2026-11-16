using Microsoft.AspNetCore.Mvc;
using Portfolio.Application.Projects;
using Portfolio.Application.Projects.Dtos;

namespace Portfolio.Api.Controllers;

/// <summary>Projects for the "Systems" section.</summary>
[ApiController]
[Route("api/projects")]
[Produces("application/json")]
public sealed class ProjectsController(IProjectService projects) : ControllerBase
{
    /// <summary>Lists all projects in display order.</summary>
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<ProjectResponse>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ProjectResponse>>> GetAll(CancellationToken ct) =>
        Ok(await projects.GetAllAsync(ct));
}
