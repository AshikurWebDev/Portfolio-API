namespace Portfolio.Domain.Entities;

/// <summary>
/// A personal access token for machine clients such as an AI agent connecting
/// to the MCP server. Created in the admin, shown once, stored only as a hash.
/// </summary>
public class ApiToken
{
    /// <summary>Primary key.</summary>
    public Guid Id { get; set; }

    /// <summary>The Identity user the token acts as.</summary>
    public Guid UserId { get; set; }

    /// <summary>A name to recognise the token, e.g. "Claude Desktop".</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>First characters of the token, shown in the admin so tokens can be told apart.</summary>
    public string Prefix { get; set; } = string.Empty;

    /// <summary>Base64 SHA-256 hash of the raw token.</summary>
    public string TokenHash { get; set; } = string.Empty;

    /// <summary>When the token was created (UTC).</summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>When the token stops working (UTC).</summary>
    public DateTimeOffset ExpiresAt { get; set; }

    /// <summary>Last time the token was used (UTC).</summary>
    public DateTimeOffset? LastUsedAt { get; set; }

    /// <summary>Set when the admin revokes the token.</summary>
    public DateTimeOffset? RevokedAt { get; set; }
}
