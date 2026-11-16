using System.ComponentModel.DataAnnotations;

namespace Portfolio.Application.Contact.Dtos;

/// <summary>A message from the website's contact form. Matches the website's <c>ContactMessage</c> type.</summary>
public sealed record CreateContactMessageRequest
{
    /// <summary>"role" or "project".</summary>
    [Required, RegularExpression("^(role|project)$")]
    public required string Kind { get; init; }

    /// <summary>Sender's name.</summary>
    [Required, MaxLength(120)]
    public required string Name { get; init; }

    /// <summary>Sender's email.</summary>
    [Required, EmailAddress, MaxLength(254)]
    public required string Email { get; init; }

    /// <summary>Company or organization.</summary>
    [MaxLength(160)]
    public string Organization { get; init; } = string.Empty;

    /// <summary>Job title or system type.</summary>
    [MaxLength(160)]
    public string Topic { get; init; } = string.Empty;

    /// <summary>Message text.</summary>
    [Required, MinLength(10), MaxLength(5000)]
    public required string Message { get; init; }

    /// <summary>Honeypot field. Real visitors never fill it; bots often do.</summary>
    [MaxLength(200)]
    public string Website { get; init; } = string.Empty;
}
