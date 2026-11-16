namespace Portfolio.Infrastructure.Options;

/// <summary>
/// Development seeding, bound from "Seed". Used only when the API runs in the
/// Development environment. Keep the password in user-secrets, never in appsettings.
/// </summary>
public sealed class SeedOptions
{
    /// <summary>Configuration section name.</summary>
    public const string Section = "Seed";

    /// <summary>Email of the admin account to create for password sign-in. Must also be in Admin:AllowedEmails.</summary>
    public string? AdminEmail { get; init; }

    /// <summary>Password for that account (hashed with Argon2id before it is stored).</summary>
    public string? AdminPassword { get; init; }
}
