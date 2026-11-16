using System.ComponentModel.DataAnnotations;

namespace Portfolio.Application.Auth.Dtos;

/// <summary>The ID token (the "credential") returned by the Google Sign-In button.</summary>
public sealed record GoogleSignInRequest
{
    /// <summary>Google ID token (a signed JWT).</summary>
    [Required, MaxLength(4096)]
    public required string IdToken { get; init; }
}
