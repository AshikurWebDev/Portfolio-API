using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Portfolio.Infrastructure.Auth;
using Portfolio.Infrastructure.Identity;
using Portfolio.Infrastructure.Options;

namespace Portfolio.Api.Extensions;

/// <summary>
/// Authentication and authorization setup.
/// Two kinds of bearer token are accepted:
/// - JWT access tokens issued by /api/auth (the admin website), and
/// - personal access tokens starting with "pat_" (AI agents on /mcp).
/// A policy scheme looks at the header and forwards to the right handler.
/// </summary>
public static class AuthenticationSetup
{
    /// <summary>Name of the policy that every admin endpoint requires.</summary>
    public const string AdminPolicy = "Admin";

    private const string SmartScheme = "Bearer-or-ApiToken";

    /// <summary>Registers JWT bearer, the API token handler and the Admin policy.</summary>
    public static IServiceCollection AddPortfolioAuth(this IServiceCollection services, IConfiguration config)
    {
        var jwt = config.GetSection(JwtOptions.Section).Get<JwtOptions>() ?? new JwtOptions();

        services.AddAuthentication(SmartScheme)
            .AddPolicyScheme(SmartScheme, "JWT or API token", o =>
            {
                o.ForwardDefaultSelector = ctx =>
                    ctx.Request.Headers.Authorization.ToString().StartsWith("Bearer " + ApiTokenService.TokenPrefix, StringComparison.Ordinal)
                        ? ApiTokenAuthenticationHandler.SchemeName
                        : JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(o =>
            {
                o.MapInboundClaims = false; // keep "sub", "email" as issued
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = jwt.Issuer,
                    ValidAudience = jwt.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.SigningKey)),
                    ValidateIssuerSigningKey = true,
                    ClockSkew = TimeSpan.FromSeconds(30),
                    NameClaimType = JwtRegisteredClaimNames.Name,
                    RoleClaimType = ClaimTypes.Role,
                };
            })
            .AddScheme<AuthenticationSchemeOptions, ApiTokenAuthenticationHandler>(ApiTokenAuthenticationHandler.SchemeName, null);

        services.AddAuthorizationBuilder()
            .AddPolicy(AdminPolicy, p => p.RequireAuthenticatedUser().RequireRole(Roles.Admin));

        return services;
    }
}
