namespace Portfolio.Application.Auth.Dtos;

/// <summary>
/// Result of a successful sign-in or refresh. The access token goes in the
/// Authorization header (Bearer). The refresh token is never in this body:
/// it is set as an HttpOnly cookie that JavaScript cannot read.
/// </summary>
/// <param name="AccessToken">Short-lived JWT.</param>
/// <param name="ExpiresAt">When the access token expires (UTC).</param>
/// <param name="User">The signed-in user.</param>
public sealed record AuthResponse(string AccessToken, DateTimeOffset ExpiresAt, CurrentUserResponse User);
