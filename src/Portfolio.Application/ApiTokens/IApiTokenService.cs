using Portfolio.Application.ApiTokens.Dtos;

namespace Portfolio.Application.ApiTokens;

/// <summary>Personal access tokens for machine clients (the MCP server). Implemented in Infrastructure.</summary>
public interface IApiTokenService
{
    /// <summary>Creates a token for the current user and returns the secret once.</summary>
    Task<CreatedApiTokenResponse> CreateAsync(CreateApiTokenRequest request, CancellationToken ct);

    /// <summary>The current user's tokens.</summary>
    Task<IReadOnlyList<ApiTokenResponse>> GetMineAsync(CancellationToken ct);

    /// <summary>Revokes one of the current user's tokens.</summary>
    Task RevokeAsync(Guid id, CancellationToken ct);
}
