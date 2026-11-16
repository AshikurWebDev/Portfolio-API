namespace Portfolio.Infrastructure.Identity;

/// <summary>Role names. Used in [Authorize(Roles = ...)] and when issuing tokens.</summary>
public static class Roles
{
    /// <summary>Can manage posts, messages and API tokens.</summary>
    public const string Admin = "Admin";
}
