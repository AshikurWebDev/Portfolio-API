using Microsoft.EntityFrameworkCore;
using Portfolio.Application.Common.Exceptions;
using Portfolio.Application.Common.Interfaces;
using Portfolio.Application.Skills.Dtos;
using Portfolio.Domain.Entities;

namespace Portfolio.Application.Skills;

/// <summary>
/// Default <see cref="ISkillService"/>. All queries go through EF Core LINQ, which sends
/// parameterized SQL, so user text (names, notes) can never change the SQL itself.
/// </summary>
public sealed class SkillService(IAppDbContext db, ISiteEvents events) : ISkillService
{
    /// <inheritdoc />
    public async Task<IReadOnlyList<SkillGroupResponse>> GetAllAsync(CancellationToken ct) =>
        await db.SkillGroups
            .AsNoTracking()
            .OrderBy(g => g.SortOrder).ThenBy(g => g.CreatedAt)
            .Select(g => new SkillGroupResponse(
                g.Id,
                g.Name,
                g.Skills
                    .OrderBy(s => s.SortOrder).ThenBy(s => s.CreatedAt)
                    .Select(s => new SkillResponse(s.Id, s.Name, s.Level, s.Icon, s.Note))
                    .ToList()))
            .ToListAsync(ct);

    /// <inheritdoc />
    public async Task<SkillGroupResponse> CreateGroupAsync(SaveSkillGroupRequest request, CancellationToken ct)
    {
        var nextOrder = await db.SkillGroups.Select(g => (int?)g.SortOrder).MaxAsync(ct) ?? -1;
        var group = new SkillGroup { Id = Guid.CreateVersion7(), Name = CleanName(request.Name, 60), SortOrder = nextOrder + 1 };
        db.SkillGroups.Add(group);
        await db.SaveChangesAsync(ct);
        await events.ContentChangedAsync(SiteTopics.Skills, ct);
        return new SkillGroupResponse(group.Id, group.Name, []);
    }

    /// <inheritdoc />
    public async Task<SkillGroupResponse> RenameGroupAsync(Guid id, SaveSkillGroupRequest request, CancellationToken ct)
    {
        var group = await db.SkillGroups.FirstOrDefaultAsync(g => g.Id == id, ct)
            ?? throw new NotFoundException("Skill group", id);
        group.Name = CleanName(request.Name, 60);
        await db.SaveChangesAsync(ct);
        await events.ContentChangedAsync(SiteTopics.Skills, ct);
        return (await GetAllAsync(ct)).First(g => g.Id == id);
    }

    /// <inheritdoc />
    public async Task DeleteGroupAsync(Guid id, CancellationToken ct)
    {
        // The foreign key cascades, so the group's skills go with it.
        var deleted = await db.SkillGroups.Where(g => g.Id == id).ExecuteDeleteAsync(ct);
        if (deleted == 0) throw new NotFoundException("Skill group", id);
        await events.ContentChangedAsync(SiteTopics.Skills, ct);
    }

    /// <inheritdoc />
    public async Task ReorderGroupsAsync(ReorderRequest request, CancellationToken ct)
    {
        var groups = await db.SkillGroups.ToListAsync(ct);
        ApplyOrder(groups, request.Ids, g => g.Id, (g, i) => g.SortOrder = i, "skill group");
        await db.SaveChangesAsync(ct);
        await events.ContentChangedAsync(SiteTopics.Skills, ct);
    }

    /// <inheritdoc />
    public async Task<SkillResponse> CreateSkillAsync(Guid groupId, SaveSkillRequest request, CancellationToken ct)
    {
        if (!await db.SkillGroups.AnyAsync(g => g.Id == groupId, ct))
            throw new NotFoundException("Skill group", groupId);

        var skill = new Skill { Id = Guid.CreateVersion7(), GroupId = groupId, SortOrder = await NextSkillOrderAsync(groupId, ct) };
        Apply(skill, request);
        db.Skills.Add(skill);
        await db.SaveChangesAsync(ct);
        await events.ContentChangedAsync(SiteTopics.Skills, ct);
        return ToResponse(skill);
    }

    /// <inheritdoc />
    public async Task<SkillResponse> UpdateSkillAsync(Guid id, SaveSkillRequest request, CancellationToken ct)
    {
        var skill = await db.Skills.FirstOrDefaultAsync(s => s.Id == id, ct)
            ?? throw new NotFoundException("Skill", id);

        if (request.GroupId is { } target && target != skill.GroupId)
        {
            if (!await db.SkillGroups.AnyAsync(g => g.Id == target, ct))
                throw new RequestValidationException(nameof(request.GroupId), "That skill group does not exist.");
            skill.GroupId = target;
            skill.SortOrder = await NextSkillOrderAsync(target, ct);
        }

        Apply(skill, request);
        await db.SaveChangesAsync(ct);
        await events.ContentChangedAsync(SiteTopics.Skills, ct);
        return ToResponse(skill);
    }

    /// <inheritdoc />
    public async Task DeleteSkillAsync(Guid id, CancellationToken ct)
    {
        var deleted = await db.Skills.Where(s => s.Id == id).ExecuteDeleteAsync(ct);
        if (deleted == 0) throw new NotFoundException("Skill", id);
        await events.ContentChangedAsync(SiteTopics.Skills, ct);
    }

    /// <inheritdoc />
    public async Task ReorderSkillsAsync(Guid groupId, ReorderRequest request, CancellationToken ct)
    {
        if (!await db.SkillGroups.AnyAsync(g => g.Id == groupId, ct))
            throw new NotFoundException("Skill group", groupId);
        var skills = await db.Skills.Where(s => s.GroupId == groupId).ToListAsync(ct);
        ApplyOrder(skills, request.Ids, s => s.Id, (s, i) => s.SortOrder = i, "skill in this group");
        await db.SaveChangesAsync(ct);
        await events.ContentChangedAsync(SiteTopics.Skills, ct);
    }

    private async Task<int> NextSkillOrderAsync(Guid groupId, CancellationToken ct) =>
        (await db.Skills.Where(s => s.GroupId == groupId).Select(s => (int?)s.SortOrder).MaxAsync(ct) ?? -1) + 1;

    /// <summary>Copies the request onto the entity after checks data annotations cannot do.</summary>
    private static void Apply(Skill skill, SaveSkillRequest request)
    {
        var icon = string.IsNullOrWhiteSpace(request.Icon) ? null : request.Icon.Trim();
        if (!SkillIcons.IsValid(icon))
            throw new RequestValidationException(nameof(request.Icon), "Pick a logo from the list, or none.");

        skill.Name = CleanName(request.Name, 80);
        skill.Level = Math.Clamp(request.Level, 0, 100);
        skill.Icon = icon;
        skill.Note = string.IsNullOrWhiteSpace(request.Note) ? null : request.Note.Trim();
    }

    private static string CleanName(string name, int max)
    {
        var n = (name ?? string.Empty).Trim();
        if (n.Length == 0 || n.Length > max)
            throw new RequestValidationException("name", $"Write a name of 1 to {max} characters.");
        return n;
    }

    /// <summary>Checks that <paramref name="ids"/> lists every item once, then sets each item's position.</summary>
    private static void ApplyOrder<T>(List<T> items, IReadOnlyList<Guid> ids, Func<T, Guid> idOf, Action<T, int> setOrder, string what)
    {
        var byId = items.ToDictionary(idOf);
        if (ids.Count != byId.Count || ids.Distinct().Count() != byId.Count || !ids.All(byId.ContainsKey))
            throw new RequestValidationException("ids", $"Send every {what} id exactly once.");
        for (var i = 0; i < ids.Count; i++) setOrder(byId[ids[i]], i);
    }

    private static SkillResponse ToResponse(Skill s) => new(s.Id, s.Name, s.Level, s.Icon, s.Note);
}
