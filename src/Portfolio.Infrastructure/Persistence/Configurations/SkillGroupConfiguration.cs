using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Portfolio.Domain.Entities;

namespace Portfolio.Infrastructure.Persistence.Configurations;

/// <summary>Table mapping for <see cref="SkillGroup"/>.</summary>
public sealed class SkillGroupConfiguration : IEntityTypeConfiguration<SkillGroup>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<SkillGroup> b)
    {
        b.ToTable("skill_groups");
        b.HasKey(g => g.Id);
        b.Property(g => g.Name).HasMaxLength(60);
        b.HasIndex(g => g.SortOrder);
        // Deleting a group deletes its skills (ON DELETE CASCADE in the database).
        b.HasMany(g => g.Skills).WithOne(s => s.Group).HasForeignKey(s => s.GroupId).OnDelete(DeleteBehavior.Cascade);
    }
}
