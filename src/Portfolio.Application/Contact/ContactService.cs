using Microsoft.EntityFrameworkCore;
using Portfolio.Application.Common.Exceptions;
using Portfolio.Application.Common.Interfaces;
using Portfolio.Application.Contact.Dtos;
using Portfolio.Domain.Entities;

namespace Portfolio.Application.Contact;

/// <summary>Default <see cref="IContactService"/>.</summary>
public sealed class ContactService(IAppDbContext db) : IContactService
{
    /// <inheritdoc />
    public async Task SubmitAsync(CreateContactMessageRequest r, CancellationToken ct)
    {
        // Bots fill every field; people never see this one. Pretend success so bots learn nothing.
        if (!string.IsNullOrEmpty(r.Website)) return;

        db.ContactMessages.Add(new ContactMessage
        {
            Id = Guid.NewGuid(),
            Kind = r.Kind,
            Name = r.Name.Trim(),
            Email = r.Email.Trim(),
            Organization = r.Organization.Trim(),
            Topic = r.Topic.Trim(),
            Message = r.Message.Trim(),
        });
        await db.SaveChangesAsync(ct);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<ContactMessageResponse>> GetAllAsync(CancellationToken ct) =>
        await db.ContactMessages.AsNoTracking()
            .OrderByDescending(m => m.CreatedAt)
            .Select(m => new ContactMessageResponse(m.Id, m.Kind, m.Name, m.Email, m.Organization, m.Topic, m.Message, m.IsRead, m.CreatedAt))
            .ToListAsync(ct);

    /// <inheritdoc />
    public async Task MarkReadAsync(Guid id, CancellationToken ct)
    {
        var message = await db.ContactMessages.FirstOrDefaultAsync(m => m.Id == id, ct)
            ?? throw new NotFoundException("Message", id);
        message.IsRead = true;
        await db.SaveChangesAsync(ct);
    }
}
