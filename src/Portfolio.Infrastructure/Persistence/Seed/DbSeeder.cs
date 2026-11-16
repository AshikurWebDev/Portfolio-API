using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Portfolio.Application.Common;
using Portfolio.Domain.Entities;
using Portfolio.Domain.Enums;
using Portfolio.Infrastructure.Identity;
using Portfolio.Infrastructure.Options;

namespace Portfolio.Infrastructure.Persistence.Seed;

/// <summary>
/// Prepares the database when the API starts in Development:
/// applies pending migrations, creates the Admin role, the seeded admin account
/// (if Seed:AdminEmail and Seed:AdminPassword are set), the two projects, the
/// three draft posts from SeedData/, and the starting Skills section (SkillSeed). Each step runs only when its data is missing,
/// so restarting the API never duplicates anything.
/// </summary>
public static class DbSeeder
{
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() },
    };

    /// <summary>Runs all seeding steps.</summary>
    public static async Task SeedAsync(IServiceProvider services, CancellationToken ct = default)
    {
        using var scope = services.CreateScope();
        var sp = scope.ServiceProvider;
        var db = sp.GetRequiredService<AppDbContext>();
        var log = sp.GetRequiredService<ILoggerFactory>().CreateLogger(typeof(DbSeeder));

        await db.Database.MigrateAsync(ct);

        var roles = sp.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
        if (!await roles.RoleExistsAsync(Roles.Admin))
            await roles.CreateAsync(new IdentityRole<Guid>(Roles.Admin) { Id = Guid.NewGuid() });

        var adminId = await SeedAdminAsync(sp, log);
        await SeedProjectsAsync(db, log, ct);
        await SeedDraftPostsAsync(db, adminId, log, ct);
        await SkillSeed.SeedAsync(db, log, ct);
    }

    private static async Task<Guid?> SeedAdminAsync(IServiceProvider sp, ILogger log)
    {
        var seed = sp.GetRequiredService<IOptions<SeedOptions>>().Value;
        if (string.IsNullOrWhiteSpace(seed.AdminEmail) || string.IsNullOrWhiteSpace(seed.AdminPassword))
            return null;

        var users = sp.GetRequiredService<UserManager<AppUser>>();
        var user = await users.FindByEmailAsync(seed.AdminEmail);
        if (user is null)
        {
            user = new AppUser
            {
                Id = Guid.NewGuid(),
                UserName = seed.AdminEmail,
                Email = seed.AdminEmail,
                EmailConfirmed = true,
                DisplayName = "Admin",
                CreatedAt = DateTimeOffset.UtcNow,
            };
            var created = await users.CreateAsync(user, seed.AdminPassword); // hashed with Argon2id
            if (!created.Succeeded)
            {
                log.LogError("Could not create the seeded admin: {Errors}", string.Join("; ", created.Errors.Select(e => e.Description)));
                return null;
            }
            log.LogInformation("Seeded admin account {Email}.", seed.AdminEmail);
        }
        if (!await users.IsInRoleAsync(user, Roles.Admin)) await users.AddToRoleAsync(user, Roles.Admin);
        return user.Id;
    }

    private static async Task SeedProjectsAsync(AppDbContext db, ILogger log, CancellationToken ct)
    {
        if (await db.Projects.AnyAsync(ct)) return;
        var path = Path.Combine(AppContext.BaseDirectory, "SeedData", "projects.json");
        if (!File.Exists(path)) return;
        var projects = JsonSerializer.Deserialize<List<Project>>(await File.ReadAllTextAsync(path, ct), Json) ?? [];
        db.Projects.AddRange(projects);
        await db.SaveChangesAsync(ct);
        log.LogInformation("Seeded {Count} projects.", projects.Count);
    }

    private static async Task SeedDraftPostsAsync(AppDbContext db, Guid? authorId, ILogger log, CancellationToken ct)
    {
        if (await db.Posts.AnyAsync(ct)) return;
        var dir = Path.Combine(AppContext.BaseDirectory, "SeedData");
        var metaPath = Path.Combine(dir, "posts.json");
        if (!File.Exists(metaPath)) return;

        var metas = JsonSerializer.Deserialize<List<SeedPost>>(await File.ReadAllTextAsync(metaPath, ct), Json) ?? [];
        foreach (var m in metas)
        {
            var bodyPath = Path.Combine(dir, "Posts", m.Slug + ".md");
            if (!File.Exists(bodyPath)) continue;
            var body = await File.ReadAllTextAsync(bodyPath, ct);
            db.Posts.Add(new Post
            {
                Id = Guid.NewGuid(),
                Slug = m.Slug,
                Title = m.Title,
                Excerpt = m.Excerpt,
                Body = body,
                Category = m.Category,
                Layer = m.Layer,
                Tags = m.Tags,
                LeadFigure = m.LeadFigure,
                Featured = m.Featured,
                Status = PostStatus.Draft, // drafts until Ashikur reviews and publishes them
                ReadingMinutes = ReadingTime.Estimate(body),
                AuthorId = authorId,
            });
        }
        await db.SaveChangesAsync(ct);
        log.LogInformation("Seeded {Count} draft posts.", metas.Count);
    }

    /// <summary>Shape of an entry in SeedData/posts.json.</summary>
    private sealed record SeedPost(
        string Slug, string Title, string Excerpt, string Category, SystemLayer Layer,
        List<string> Tags, string? LeadFigure, bool Featured);
}
