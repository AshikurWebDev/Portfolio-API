using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Portfolio.Domain.Entities;

namespace Portfolio.Infrastructure.Persistence.Configurations;

/// <summary>Table mapping for <see cref="CvDocument"/>.</summary>
public sealed class CvDocumentConfiguration : IEntityTypeConfiguration<CvDocument>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<CvDocument> b)
    {
        b.ToTable("cv_documents");
        b.HasKey(c => c.Id);
        b.Property(c => c.OriginalFileName).HasMaxLength(200);
        b.Property(c => c.Data).HasColumnType("bytea");
        b.Property(c => c.Version).HasMaxLength(16);
    }
}
