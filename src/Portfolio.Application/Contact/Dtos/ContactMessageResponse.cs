namespace Portfolio.Application.Contact.Dtos;

/// <summary>A contact message as shown in the admin.</summary>
/// <param name="Id">Message id.</param>
/// <param name="Kind">"role" or "project".</param>
/// <param name="Name">Sender's name.</param>
/// <param name="Email">Sender's email.</param>
/// <param name="Organization">Company or organization.</param>
/// <param name="Topic">Job title or system type.</param>
/// <param name="Message">Message text.</param>
/// <param name="IsRead">Whether the admin has read it.</param>
/// <param name="CreatedAt">Received (UTC).</param>
public sealed record ContactMessageResponse(
    Guid Id,
    string Kind,
    string Name,
    string Email,
    string Organization,
    string Topic,
    string Message,
    bool IsRead,
    DateTimeOffset CreatedAt);
