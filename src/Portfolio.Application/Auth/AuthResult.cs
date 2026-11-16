using Portfolio.Application.Auth.Dtos;

namespace Portfolio.Application.Auth;

/// <summary>What the auth service hands to the controller: the response body plus the refresh token for the cookie.</summary>
/// <param name="Response">Body returned to the client.</param>
/// <param name="RefreshToken">Raw refresh token, to be written to an HttpOnly cookie only.</param>
/// <param name="RefreshExpiresAt">When the refresh token expires.</param>
public sealed record AuthResult(AuthResponse Response, string RefreshToken, DateTimeOffset RefreshExpiresAt);
