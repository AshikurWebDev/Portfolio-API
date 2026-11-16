using System.ComponentModel.DataAnnotations;

namespace Portfolio.Infrastructure.Options;

/// <summary>
/// Settings for the access tokens (JWT) the API issues. Bound from the "Jwt"
/// configuration section. <see cref="SigningKey"/> is a secret: set it with
/// dotnet user-secrets in development and an environment variable in production.
/// </summary>
public sealed class JwtOptions
{
    /// <summary>Configuration section name.</summary>
    public const string Section = "Jwt";

    /// <summary>Who issues the token (this API), e.g. "portfolio-api".</summary>
    [Required]
    public string Issuer { get; init; } = string.Empty;

    /// <summary>Who the token is for, e.g. "portfolio-web".</summary>
    [Required]
    public string Audience { get; init; } = string.Empty;

    /// <summary>HMAC-SHA256 key, at least 32 characters.</summary>
    [Required, MinLength(32)]
    public string SigningKey { get; init; } = string.Empty;

    /// <summary>Access token lifetime in minutes. Short, because it cannot be revoked.</summary>
    [Range(1, 120)]
    public int AccessTokenMinutes { get; init; } = 15;

    /// <summary>Refresh token lifetime in days.</summary>
    [Range(1, 90)]
    public int RefreshTokenDays { get; init; } = 14;
}
