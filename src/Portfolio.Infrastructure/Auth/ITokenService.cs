using Portfolio.Infrastructure.Identity;

namespace Portfolio.Infrastructure.Auth;

/// <summary>Issues access tokens (JWT) for a user.</summary>
public interface ITokenService
{
    /// <summary>Creates a signed JWT carrying the user's id, email, name and roles.</summary>
    (string Token, DateTimeOffset ExpiresAt) CreateAccessToken(AppUser user, IReadOnlyList<string> roles);
}
