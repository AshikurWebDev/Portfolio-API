using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Api.Extensions;
using Portfolio.Application.Overview;
using Portfolio.Application.Overview.Dtos;

namespace Portfolio.Api.Controllers.Admin;

/// <summary>Numbers for the admin dashboard's home page.</summary>
[ApiController]
[Route("api/admin/overview")]
[Produces("application/json")]
[Authorize(Policy = AuthenticationSetup.AdminPolicy)]
public sealed class AdminOverviewController(IAdminOverviewService overview) : ControllerBase
{
    /// <summary>Counts of posts, design items, unread messages and skills, plus the CV status.</summary>
    [HttpGet]
    [ProducesResponseType<AdminOverviewResponse>(StatusCodes.Status200OK)]
    public async Task<ActionResult<AdminOverviewResponse>> Get(CancellationToken ct) =>
        Ok(await overview.GetAsync(ct));
}
