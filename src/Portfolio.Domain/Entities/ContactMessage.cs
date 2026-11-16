using Portfolio.Domain.Common;

namespace Portfolio.Domain.Entities;

/// <summary>A message sent through the contact form on the website.</summary>
public class ContactMessage : AuditableEntity
{
    /// <summary>Primary key.</summary>
    public Guid Id { get; set; }

    /// <summary>"role" (a job offer) or "project" (a project brief).</summary>
    public string Kind { get; set; } = string.Empty;

    /// <summary>Sender's name.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Sender's email address.</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>Company (role) or organization (project).</summary>
    public string Organization { get; set; } = string.Empty;

    /// <summary>Job title (role) or system type (project).</summary>
    public string Topic { get; set; } = string.Empty;

    /// <summary>The message text.</summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>True once the admin has read it.</summary>
    public bool IsRead { get; set; }
}
