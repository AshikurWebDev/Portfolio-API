using System.ComponentModel.DataAnnotations;

namespace Portfolio.Application.Auth.Dtos;

/// <summary>Email and password sign-in for the admin account (passwords are hashed with Argon2id).</summary>
public sealed record PasswordSignInRequest
{
    /// <summary>Admin email.</summary>
    [Required, EmailAddress]
    public required string Email { get; init; }

    /// <summary>Password.</summary>
    [Required, MaxLength(256)]
    public required string Password { get; init; }
}
