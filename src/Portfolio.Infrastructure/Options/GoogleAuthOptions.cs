namespace Portfolio.Infrastructure.Options;

/// <summary>
/// Google Sign-In settings, bound from "Authentication:Google". The client id is
/// created in Google Cloud Console (APIs &amp; Services, Credentials, OAuth client
/// ID, type "Web application"). It is not secret, but keep it in configuration.
/// </summary>
public sealed class GoogleAuthOptions
{
    /// <summary>Configuration section name.</summary>
    public const string Section = "Authentication:Google";

    /// <summary>OAuth 2.0 client id, e.g. "1234-abc.apps.googleusercontent.com". Empty disables Google sign-in.</summary>
    public string ClientId { get; init; } = string.Empty;
}
