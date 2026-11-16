using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Portfolio.Domain.Entities;

namespace Portfolio.Infrastructure.Persistence.Seed;

/// <summary>
/// The starting Skills section, copied from the website's static content (web/src/content/skills.ts).
/// Added once, when the skills table is empty; after that the admin dashboard owns the data.
/// </summary>
internal static class SkillSeed
{
    private sealed record SeedSkill(string Name, int Level, string? Icon, string? Note = null);

    private static readonly (string Group, SeedSkill[] Skills)[] Data =
    [
        ("Backend · .NET",
        [
            new("ASP.NET Core Web API", 100, "dotnet"),
            new("Entity Framework Core", 80, "dotnet"),
            new("ASP.NET MVC 5 / Web API", 80, "dotnet"),
            new("C#", 100, "csharp"),
        ]),
        ("Backend · JavaScript",
        [
            new("Node.js", 65, "nodejs"),
            new("Express", 65, "express"),
        ]),
        ("Desktop",
        [
            new("Windows Forms", 75, "desktop"),
            new("WPF", 70, "desktop"),
        ]),
        ("Databases",
        [
            new("MS SQL Server", 100, "mssql"),
            new("PostgreSQL", 75, "postgresql"),
            new("MongoDB", 60, "mongodb"),
        ]),
        ("Frontend",
        [
            new("React + TypeScript", 100, "react"),
            new("Angular", 50, "angular", "normal knowledge"),
        ]),
        ("AI engineering",
        [
            new("AI agents", 70, "agent"),
            new("MCP (Model Context Protocol) servers", 70, "mcp"),
            new("Agent skills", 70, "ai"),
            new("Prompt engineering", 80, "prompt"),
            new("Agent harness", 65, "claude"),
        ]),
    ];

    /// <summary>Adds the starting cards and skills when there are none.</summary>
    public static async Task SeedAsync(AppDbContext db, ILogger log, CancellationToken ct)
    {
        if (await db.SkillGroups.AnyAsync(ct)) return;

        for (var g = 0; g < Data.Length; g++)
        {
            var (name, skills) = Data[g];
            db.SkillGroups.Add(new SkillGroup
            {
                Id = Guid.CreateVersion7(),
                Name = name,
                SortOrder = g,
                Skills = skills.Select((s, i) => new Skill
                {
                    Id = Guid.CreateVersion7(),
                    Name = s.Name,
                    Level = s.Level,
                    Icon = s.Icon,
                    Note = s.Note,
                    SortOrder = i,
                }).ToList(),
            });
        }
        await db.SaveChangesAsync(ct);
        log.LogInformation("Seeded {Count} skill groups.", Data.Length);
    }
}
