using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Api.Extensions;
using Portfolio.Application.ApiTokens;
using Portfolio.Application.ApiTokens.Dtos;

namespace Portfolio.Api.Controllers.Admin;

/// <summary>Personal access tokens for AI agents that connect to the MCP server at /mcp.</summary>
[ApiController]
[Route("api/admin/api-tokens")]
[Produces("application/json")]
[Authorize(Policy = AuthenticationSetup.AdminPolicy)]
public sealed class AdminApiTokensController(IApiTokenService tokens) : ControllerBase
{
    /// <summary>Lists your tokens (never the secrets).</summary>
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<ApiTokenResponse>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ApiTokenResponse>>> GetMine(CancellationToken ct) =>
        Ok(await tokens.GetMineAsync(ct));

    /// <summary>Creates a token. The secret is in the response once; copy it into your AI client.</summary>
    [HttpPost]
    [ProducesResponseType<CreatedApiTokenResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CreatedApiTokenResponse>> Create(CreateApiTokenRequest request, CancellationToken ct)
    {
        var created = await tokens.CreateAsync(request, ct);
        return Created($"/api/admin/api-tokens/{created.Details.Id}", created);
    }

    /// <summary>Revokes a token immediately.</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Revoke(Guid id, CancellationToken ct)
    {
        await tokens.RevokeAsync(id, ct);
        return NoContent();
    }
}
