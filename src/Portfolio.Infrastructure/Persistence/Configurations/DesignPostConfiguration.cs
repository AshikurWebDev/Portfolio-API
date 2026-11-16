using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Portfolio.Domain.Entities;

namespace Portfolio.Infrastructure.Persistence.Configurations;

/// <summary>Table mapping for <see cref="DesignPost"/>.</summary>
public sealed class DesignPostConfiguration : IEntityTypeConfiguration<DesignPost>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<DesignPost> b)
    {
        b.ToTable("design_posts");
        b.HasKey(d => d.Id);
        b.Property(d => d.Caption).HasMaxLength(200);
        b.Property(d => d.Link).HasMaxLength(500);
        // PostgreSQL "bytea". List queries project to DTOs, so these bytes are only read by the image endpoint.
        b.Property(d => d.ImageData).HasColumnType("bytea");
        b.Property(d => d.ImageVersion).HasMaxLength(16);
        b.HasIndex(d => new { d.IsVisible, d.SortOrder });
    }
}
