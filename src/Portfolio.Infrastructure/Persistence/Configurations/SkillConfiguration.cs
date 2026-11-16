using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Portfolio.Domain.Entities;

namespace Portfolio.Infrastructure.Persistence.Configurations;

/// <summary>Table mapping for <see cref="Skill"/>.</summary>
public sealed class SkillConfiguration : IEntityTypeConfiguration<Skill>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Skill> b)
    {
        b.ToTable("skills", t => t.HasCheckConstraint("ck_skills_level", "\"Level\" BETWEEN 0 AND 100"));
        b.HasKey(s => s.Id);
        b.Property(s => s.Name).HasMaxLength(80);
        b.Property(s => s.Icon).HasMaxLength(40);
        b.Property(s => s.Note).HasMaxLength(60);
        b.HasIndex(s => new { s.GroupId, s.SortOrder });
    }
}
