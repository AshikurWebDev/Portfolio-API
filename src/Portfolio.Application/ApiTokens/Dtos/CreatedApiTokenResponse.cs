namespace Portfolio.Application.ApiTokens.Dtos;

/// <summary>A newly created token. <paramref name="Token"/> is shown this one time only; store it in your AI client's settings.</summary>
/// <param name="Token">The secret token (starts with "pat_").</param>
/// <param name="Details">The token's listing details.</param>
public sealed record CreatedApiTokenResponse(string Token, ApiTokenResponse Details);
