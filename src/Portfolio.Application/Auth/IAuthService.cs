using Portfolio.Application.Auth.Dtos;

namespace Portfolio.Application.Auth;

/// <summary>
/// Sign-in, token refresh and sign-out. Implemented in Infrastructure on top of
/// ASP.NET Core Identity. Only emails on the admin allowlist may sign in.
/// </summary>
public interface IAuthService
{
    /// <summary>Validates a Google ID token and signs the admin in.</summary>
    Task<AuthResult> SignInWithGoogleAsync(GoogleSignInRequest request, CancellationToken ct);

    /// <summary>Signs in with email and password (Argon2id-hashed).</summary>
    Task<AuthResult> SignInWithPasswordAsync(PasswordSignInRequest request, CancellationToken ct);

    /// <summary>Exchanges a refresh token for a new access token and a new refresh token (rotation).</summary>
    Task<AuthResult> RefreshAsync(string refreshToken, CancellationToken ct);

    /// <summary>Revokes a refresh token. Unknown tokens are ignored.</summary>
    Task SignOutAsync(string? refreshToken, CancellationToken ct);

    /// <summary>The user for an id, or null.</summary>
    Task<CurrentUserResponse?> GetUserAsync(Guid userId, CancellationToken ct);
}
