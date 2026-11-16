using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Portfolio.Application.Common;
using Portfolio.Domain.Entities;

namespace Portfolio.Infrastructure.Persistence.Configurations;

/// <summary>Table mapping for <see cref="Post"/>.</summary>
public sealed class PostConfiguration : IEntityTypeConfiguration<Post>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Post> b)
    {
        b.ToTable("posts");
        b.HasKey(p => p.Id);
        b.Property(p => p.Slug).HasMaxLength(SlugRules.MaxLength).IsRequired();
        b.HasIndex(p => p.Slug).IsUnique();
        b.Property(p => p.Title).HasMaxLength(200).IsRequired();
        b.Property(p => p.Excerpt).HasMaxLength(400).IsRequired();
        b.Property(p => p.Body).IsRequired();
        b.Property(p => p.Category).HasMaxLength(60).IsRequired();
        // Enums stored as readable text ("Draft", "Api") rather than numbers.
        b.Property(p => p.Layer).HasConversion<string>().HasMaxLength(10);
        b.Property(p => p.Status).HasConversion<string>().HasMaxLength(12);
        b.Property(p => p.Tags).HasColumnType("text[]");
        b.Property(p => p.LeadFigure).HasMaxLength(60);
        b.Property(p => p.CoverUrl).HasMaxLength(500);
        b.Property(p => p.CoverAlt).HasMaxLength(200);
        // The public list filters by status and sorts by date.
        b.HasIndex(p => new { p.Status, p.PublishedOn });
    }
}
