namespace Portfolio.Domain.Entities;

/// <summary>
/// A long-lived refresh token, used to get a new short-lived access token (JWT)
/// without signing in again. Only a SHA-256 hash of the token is stored, so a
/// leaked database cannot be used to sign in.
/// </summary>
public class RefreshToken
{
    /// <summary>Primary key.</summary>
    public Guid Id { get; set; }

    /// <summary>The Identity user the token belongs to.</summary>
    public Guid UserId { get; set; }

    /// <summary>Base64 SHA-256 hash of the raw token.</summary>
    public string TokenHash { get; set; } = string.Empty;

    /// <summary>When the token was issued (UTC).</summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>When the token stops working (UTC).</summary>
    public DateTimeOffset ExpiresAt { get; set; }

    /// <summary>Set when the token is used (rotated) or the user signs out.</summary>
    public DateTimeOffset? RevokedAt { get; set; }

    /// <summary>True while the token can still be used.</summary>
    public bool IsActive(DateTimeOffset now) => RevokedAt is null && ExpiresAt > now;
}
