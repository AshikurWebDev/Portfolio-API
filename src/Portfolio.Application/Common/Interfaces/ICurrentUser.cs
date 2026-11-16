namespace Portfolio.Application.Common.Interfaces;

/// <summary>
/// The signed-in user for the current request (from the JWT or API token).
/// Implemented in the Api project on top of <c>HttpContext.User</c>.
/// </summary>
public interface ICurrentUser
{
    /// <summary>The user's id, or null for anonymous requests.</summary>
    Guid? UserId { get; }

    /// <summary>The user's email, or null for anonymous requests.</summary>
    string? Email { get; }
}
