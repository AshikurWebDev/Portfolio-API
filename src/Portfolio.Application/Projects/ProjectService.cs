using Microsoft.EntityFrameworkCore;
using Portfolio.Application.Common.Interfaces;
using Portfolio.Application.Projects.Dtos;
using Portfolio.Domain.Entities;
using Portfolio.Domain.Enums;

namespace Portfolio.Application.Projects;

/// <summary>Default <see cref="IProjectService"/>.</summary>
public sealed class ProjectService(IAppDbContext db) : IProjectService
{
    /// <inheritdoc />
    public async Task<IReadOnlyList<ProjectResponse>> GetAllAsync(CancellationToken ct)
    {
        var projects = await db.Projects.AsNoTracking().OrderBy(p => p.SortOrder).ToListAsync(ct);
        return projects.Select(ToResponse).ToList();
    }

    /// <summary>The website spells statuses in kebab case ("in-development").</summary>
    private static string StatusText(ProjectStatus s) => s switch
    {
        ProjectStatus.Live => "live",
        ProjectStatus.InDevelopment => "in-development",
        _ => "completed",
    };

    private static ProjectResponse ToResponse(Project p) => new(
        p.Id,
        p.Title,
        p.Tags,
        StatusText(p.Status),
        p.StatusLabel,
        p.Facts.Select(f => new ProjectFactResponse(f.Label, f.Value)).ToList(),
        p.Summary,
        p.Highlights.Select(h => new ProjectHighlightResponse(h.Text, h.InProgress ? true : null)).ToList(),
        new ProjectMediaResponse(p.Media.Type, p.Media.Src, p.Media.Poster, p.Media.Alt, p.Media.FragmentId),
        p.Tone,
        p.DemoUrl,
        p.CodeUrl,
        p.CaseStudyUrl,
        p.SortOrder);
}
