using Microsoft.AspNetCore.Identity;

namespace Portfolio.Infrastructure.Identity;

/// <summary>
/// An account managed by ASP.NET Core Identity (table "AspNetUsers"). Uses a Guid key.
/// Accounts are created by Google sign-in (for allowlisted emails) or by the
/// development seeder; there is no public registration.
/// </summary>
public class AppUser : IdentityUser<Guid>
{
    /// <summary>Name to show in the admin.</summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>Profile picture URL from Google, if any.</summary>
    public string? AvatarUrl { get; set; }

    /// <summary>When the account was created (UTC).</summary>
    public DateTimeOffset CreatedAt { get; set; }
}
