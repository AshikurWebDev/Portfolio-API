using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Portfolio.Domain.Entities;

namespace Portfolio.Infrastructure.Persistence.Configurations;

/// <summary>Table mapping for <see cref="ContactMessage"/>.</summary>
public sealed class ContactMessageConfiguration : IEntityTypeConfiguration<ContactMessage>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<ContactMessage> b)
    {
        b.ToTable("contact_messages");
        b.HasKey(m => m.Id);
        b.Property(m => m.Kind).HasMaxLength(10);
        b.Property(m => m.Name).HasMaxLength(120);
        b.Property(m => m.Email).HasMaxLength(254);
        b.Property(m => m.Organization).HasMaxLength(160);
        b.Property(m => m.Topic).HasMaxLength(160);
        b.Property(m => m.Message).HasMaxLength(5000);
        b.HasIndex(m => m.CreatedAt);
    }
}
