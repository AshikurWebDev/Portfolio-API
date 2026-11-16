using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Portfolio.Infrastructure.Identity;
using Portfolio.Infrastructure.Options;
using Portfolio.Infrastructure.Persistence;

namespace Portfolio.Infrastructure.Auth;

/// <summary>
/// Authenticates "Authorization: Bearer pat_..." headers (personal access tokens,
/// used by AI agents on the MCP server). Builds the same claims a JWT would carry,
/// so [Authorize(Roles = "Admin")] works the same for both.
/// </summary>
public sealed class ApiTokenAuthenticationHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    AppDbContext db,
    UserManager<AppUser> users,
    IOptions<AdminOptions> admin,
    TimeProvider clock) : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    /// <summary>Name of this authentication scheme.</summary>
    public const string SchemeName = "ApiToken";

    /// <inheritdoc />
    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var header = Request.Headers.Authorization.ToString();
        if (!header.StartsWith("Bearer " + ApiTokenService.TokenPrefix, StringComparison.Ordinal))
            return AuthenticateResult.NoResult();

        var secret = header["Bearer ".Length..].Trim();
        var hash = TokenHasher.Hash(secret);
        var now = clock.GetUtcNow();
        var token = await db.ApiTokens.FirstOrDefaultAsync(t => t.TokenHash == hash, Context.RequestAborted);
        if (token is null || token.RevokedAt is not null || token.ExpiresAt <= now)
            return AuthenticateResult.Fail("Invalid or expired API token.");

        var user = await users.FindByIdAsync(token.UserId.ToString());
        if (user is null || !admin.Value.IsAllowed(user.Email))
            return AuthenticateResult.Fail("Token owner is no longer allowed.");

        // Record use at most once a minute to avoid a write on every MCP call.
        if (token.LastUsedAt is null || now - token.LastUsedAt > TimeSpan.FromMinutes(1))
        {
            token.LastUsedAt = now;
            await db.SaveChangesAsync(Context.RequestAborted);
        }

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new(JwtRegisteredClaimNames.Name, user.DisplayName),
            new("token_name", token.Name),
        };
        claims.AddRange((await users.GetRolesAsync(user)).Select(r => new Claim(ClaimTypes.Role, r)));

        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, SchemeName));
        return AuthenticateResult.Success(new AuthenticationTicket(principal, SchemeName));
    }
}
