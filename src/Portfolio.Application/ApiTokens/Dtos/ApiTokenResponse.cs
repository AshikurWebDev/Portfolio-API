namespace Portfolio.Application.ApiTokens.Dtos;

/// <summary>A personal access token as listed in the admin (never the secret itself).</summary>
/// <param name="Id">Token id.</param>
/// <param name="Name">Token name.</param>
/// <param name="Prefix">First characters, to tell tokens apart.</param>
/// <param name="CreatedAt">Created (UTC).</param>
/// <param name="ExpiresAt">Expires (UTC).</param>
/// <param name="LastUsedAt">Last used (UTC), if ever.</param>
/// <param name="Revoked">True once revoked.</param>
public sealed record ApiTokenResponse(Guid Id, string Name, string Prefix, DateTimeOffset CreatedAt, DateTimeOffset ExpiresAt, DateTimeOffset? LastUsedAt, bool Revoked);
