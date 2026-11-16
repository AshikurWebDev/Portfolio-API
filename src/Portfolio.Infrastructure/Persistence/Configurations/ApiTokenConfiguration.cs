using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Portfolio.Domain.Entities;
using Portfolio.Infrastructure.Identity;

namespace Portfolio.Infrastructure.Persistence.Configurations;

/// <summary>Table mapping for <see cref="ApiToken"/>. Tokens are deleted with their user.</summary>
public sealed class ApiTokenConfiguration : IEntityTypeConfiguration<ApiToken>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<ApiToken> b)
    {
        b.ToTable("api_tokens");
        b.HasKey(t => t.Id);
        b.Property(t => t.Name).HasMaxLength(80).IsRequired();
        b.Property(t => t.Prefix).HasMaxLength(16).IsRequired();
        b.Property(t => t.TokenHash).HasMaxLength(64).IsRequired();
        b.HasIndex(t => t.TokenHash).IsUnique();
        b.HasOne<AppUser>().WithMany().HasForeignKey(t => t.UserId).OnDelete(DeleteBehavior.Cascade);
    }
}
