using Portfolio.Application.Projects.Dtos;

namespace Portfolio.Application.Projects;

/// <summary>Read access to portfolio projects.</summary>
public interface IProjectService
{
    /// <summary>All projects in display order.</summary>
    Task<IReadOnlyList<ProjectResponse>> GetAllAsync(CancellationToken ct);
}
