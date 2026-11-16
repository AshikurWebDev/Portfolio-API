using Microsoft.EntityFrameworkCore;
using Portfolio.Application.Common.Interfaces;
using Portfolio.Application.Overview.Dtos;
using Portfolio.Domain.Enums;

namespace Portfolio.Application.Overview;

/// <summary>Default <see cref="IAdminOverviewService"/>: a handful of COUNT queries, no rows loaded.</summary>
public sealed class AdminOverviewService(IAppDbContext db) : IAdminOverviewService
{
    /// <inheritdoc />
    public async Task<AdminOverviewResponse> GetAsync(CancellationToken ct)
    {
        // One DbContext cannot run queries in parallel, so these run one after another. Each is a cheap COUNT.
        var published = await db.Posts.CountAsync(p => p.Status == PostStatus.Published, ct);
        var drafts = await db.Posts.CountAsync(p => p.Status == PostStatus.Draft, ct);
        var design = await db.DesignPosts.CountAsync(ct);
        var unread = await db.ContactMessages.CountAsync(m => !m.IsRead, ct);
        var groups = await db.SkillGroups.CountAsync(ct);
        var skills = await db.Skills.CountAsync(ct);
        var cvAt = await db.CvDocuments.OrderByDescending(c => c.UpdatedAt).Select(c => (DateTimeOffset?)c.UpdatedAt).FirstOrDefaultAsync(ct);
        return new AdminOverviewResponse(published, drafts, design, unread, groups, skills, cvAt is not null, cvAt);
    }
}
