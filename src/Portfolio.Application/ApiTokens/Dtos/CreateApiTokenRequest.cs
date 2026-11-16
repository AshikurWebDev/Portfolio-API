using System.ComponentModel.DataAnnotations;

namespace Portfolio.Application.ApiTokens.Dtos;

/// <summary>Request to create a personal access token, e.g. for an AI agent using the MCP server.</summary>
public sealed record CreateApiTokenRequest
{
    /// <summary>A name to recognise the token, e.g. "Claude Desktop".</summary>
    [Required, MaxLength(80)]
    public required string Name { get; init; }

    /// <summary>Days until the token expires (1 to 365).</summary>
    [Range(1, 365)]
    public int ExpiresInDays { get; init; } = 90;
}
