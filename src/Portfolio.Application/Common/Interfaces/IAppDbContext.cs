using Microsoft.EntityFrameworkCore;
using Portfolio.Domain.Entities;

namespace Portfolio.Application.Common.Interfaces;

/// <summary>
/// The parts of the database the application services may use.
/// Implemented by <c>AppDbContext</c> in the Infrastructure project, so services
/// depend on this interface and never on PostgreSQL or Identity directly.
/// </summary>
public interface IAppDbContext
{
    /// <summary>Blog posts.</summary>
    DbSet<Post> Posts { get; }

    /// <summary>Portfolio projects.</summary>
    DbSet<Project> Projects { get; }

    /// <summary>Messages from the contact form.</summary>
    DbSet<ContactMessage> ContactMessages { get; }

    /// <summary>Graphic design items for the home page gallery.</summary>
    DbSet<DesignPost> DesignPosts { get; }

    /// <summary>Cards in the Skills section.</summary>
    DbSet<SkillGroup> SkillGroups { get; }

    /// <summary>Skills with their levels.</summary>
    DbSet<Skill> Skills { get; }

    /// <summary>The CV PDF (at most one row).</summary>
    DbSet<CvDocument> CvDocuments { get; }

    /// <summary>Saves all pending changes in one transaction.</summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
