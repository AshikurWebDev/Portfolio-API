using Portfolio.Application.Contact.Dtos;

namespace Portfolio.Application.Contact;

/// <summary>Contact form use cases.</summary>
public interface IContactService
{
    /// <summary>Stores a message. Messages with the honeypot filled are dropped silently.</summary>
    Task SubmitAsync(CreateContactMessageRequest request, CancellationToken ct);

    /// <summary>All messages for the admin, newest first.</summary>
    Task<IReadOnlyList<ContactMessageResponse>> GetAllAsync(CancellationToken ct);

    /// <summary>Marks a message as read.</summary>
    Task MarkReadAsync(Guid id, CancellationToken ct);
}
