namespace Portfolio.Infrastructure.Auth;

/// <summary>The verified facts taken from a Google ID token.</summary>
/// <param name="Subject">Google's stable user id ("sub").</param>
/// <param name="Email">Email address.</param>
/// <param name="EmailVerified">Whether Google has verified the email.</param>
/// <param name="Name">Display name.</param>
/// <param name="Picture">Profile picture URL.</param>
public sealed record GoogleIdentity(string Subject, string Email, bool EmailVerified, string? Name, string? Picture);
