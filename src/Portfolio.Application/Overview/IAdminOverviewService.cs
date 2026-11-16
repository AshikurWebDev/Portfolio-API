using Portfolio.Application.Overview.Dtos;

namespace Portfolio.Application.Overview;

/// <summary>Numbers for the admin dashboard's home page.</summary>
public interface IAdminOverviewService
{
    /// <summary>Counts of posts, design items, unread messages, skills, and the CV status.</summary>
    Task<AdminOverviewResponse> GetAsync(CancellationToken ct);
}
