using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Portfolio.Application.Common.Interfaces;
using Portfolio.Domain.Common;
using Portfolio.Domain.Entities;
using Portfolio.Infrastructure.Identity;

namespace Portfolio.Infrastructure.Persistence;

/// <summary>
/// The EF Core database context (code first). Includes the ASP.NET Core Identity
/// tables (users, roles, logins) plus the portfolio's own tables. Table and column
/// mappings live in the Configurations folder, one class per entity.
///
/// Change the model, then create a migration:
/// <code>dotnet ef migrations add &lt;Name&gt; -p src/Portfolio.Infrastructure -s src/Portfolio.Api -o Persistence/Migrations</code>
/// </summary>
public sealed class AppDbContext(DbContextOptions<AppDbContext> options, TimeProvider clock)
    : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>(options), IAppDbContext
{
    /// <inheritdoc />
    public DbSet<Post> Posts => Set<Post>();

    /// <inheritdoc />
    public DbSet<Project> Projects => Set<Project>();

    /// <inheritdoc />
    public DbSet<ContactMessage> ContactMessages => Set<ContactMessage>();

    /// <inheritdoc />
    public DbSet<DesignPost> DesignPosts => Set<DesignPost>();

    /// <inheritdoc />
    public DbSet<SkillGroup> SkillGroups => Set<SkillGroup>();

    /// <inheritdoc />
    public DbSet<Skill> Skills => Set<Skill>();

    /// <inheritdoc />
    public DbSet<CvDocument> CvDocuments => Set<CvDocument>();

    /// <summary>Refresh tokens (hashed).</summary>
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    /// <summary>Personal access tokens for MCP clients (hashed).</summary>
    public DbSet<ApiToken> ApiTokens => Set<ApiToken>();

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder); // Identity tables first
        builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }

    /// <summary>Stamps CreatedAt / UpdatedAt on every <see cref="AuditableEntity"/> before saving.</summary>
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var now = clock.GetUtcNow();
        foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = now;
                entry.Entity.UpdatedAt = now;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = now;
            }
        }
        return base.SaveChangesAsync(cancellationToken);
    }
}
