namespace Portfolio.Infrastructure.Auth;

/// <summary>
/// Checks a Google ID token. Behind an interface so tests can replace Google with
/// a fake: a real Google sign-in cannot run inside an automated test.
/// </summary>
public interface IGoogleTokenValidator
{
    /// <summary>Returns the verified identity, or null when the token is invalid, expired or for another app.</summary>
    Task<GoogleIdentity?> ValidateAsync(string idToken, CancellationToken ct);
}
