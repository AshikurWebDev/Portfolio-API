using Microsoft.Extensions.DependencyInjection;
using Portfolio.Application.Contact;
using Portfolio.Application.Cv;
using Portfolio.Application.Design;
using Portfolio.Application.Overview;
using Portfolio.Application.Posts;
using Portfolio.Application.Projects;
using Portfolio.Application.Skills;

namespace Portfolio.Application;

/// <summary>Registers the application services. Called from Program.cs.</summary>
public static class DependencyInjection
{
    /// <summary>Adds every application service with its interface (scoped: one instance per HTTP request).</summary>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IPostService, PostService>();
        services.AddScoped<IProjectService, ProjectService>();
        services.AddScoped<IContactService, ContactService>();
        services.AddScoped<IDesignPostService, DesignPostService>();
        services.AddScoped<ISkillService, SkillService>();
        services.AddScoped<ICvService, CvService>();
        services.AddScoped<IAdminOverviewService, AdminOverviewService>();
        services.AddSingleton(TimeProvider.System);
        return services;
    }
}
