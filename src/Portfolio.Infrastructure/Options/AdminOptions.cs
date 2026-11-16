namespace Portfolio.Infrastructure.Options;

/// <summary>
/// Who may manage the blog, bound from "Admin". Google sign-in proves who someone
/// is, not that they are allowed in, so only emails listed here can sign in.
/// There is no public registration endpoint.
/// </summary>
public sealed class AdminOptions
{
    /// <summary>Configuration section name.</summary>
    public const string Section = "Admin";

    /// <summary>Emails allowed to sign in as admin (case-insensitive).</summary>
    public string[] AllowedEmails { get; init; } = [];

    /// <summary>True when <paramref name="email"/> is on the allowlist.</summary>
    public bool IsAllowed(string? email) =>
        !string.IsNullOrWhiteSpace(email) && AllowedEmails.Any(e => string.Equals(e.Trim(), email.Trim(), StringComparison.OrdinalIgnoreCase));
}
