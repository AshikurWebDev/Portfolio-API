namespace Portfolio.Application.Auth.Dtos;

/// <summary>The signed-in user.</summary>
/// <param name="Id">User id.</param>
/// <param name="Email">Email.</param>
/// <param name="DisplayName">Name to show.</param>
/// <param name="AvatarUrl">Profile picture from Google, if any.</param>
/// <param name="Roles">Roles, e.g. "Admin".</param>
public sealed record CurrentUserResponse(Guid Id, string Email, string DisplayName, string? AvatarUrl, IReadOnlyList<string> Roles);
