using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Portfolio.Domain.Entities;

namespace Portfolio.Infrastructure.Persistence.Configurations;

/// <summary>Table mapping for <see cref="Project"/>. Facts, highlights and media are JSON columns.</summary>
public sealed class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Project> b)
    {
        b.ToTable("projects");
        b.HasKey(p => p.Id);
        b.Property(p => p.Id).HasMaxLength(80);
        b.Property(p => p.Title).HasMaxLength(160).IsRequired();
        b.Property(p => p.Tags).HasColumnType("text[]");
        b.Property(p => p.Status).HasConversion<string>().HasMaxLength(20);
        b.Property(p => p.StatusLabel).HasMaxLength(60);
        b.Property(p => p.Tone).HasMaxLength(10);
        b.OwnsMany(p => p.Facts, o => o.ToJson());
        b.OwnsMany(p => p.Highlights, o => o.ToJson());
        b.OwnsOne(p => p.Media, o => o.ToJson());
    }
}
