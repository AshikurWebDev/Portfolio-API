using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Portfolio.Application.ApiTokens;
using Portfolio.Application.Auth;
using Portfolio.Application.Common.Images;
using Portfolio.Application.Common.Interfaces;
using Portfolio.Infrastructure.Auth;
using Portfolio.Infrastructure.Identity;
using Portfolio.Infrastructure.Images;
using Portfolio.Infrastructure.Options;
using Portfolio.Infrastructure.Persistence;

namespace Portfolio.Infrastructure;

/// <summary>Registers database, Identity and auth services. Called from Program.cs.</summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds PostgreSQL (EF Core + Npgsql), ASP.NET Core Identity with the Argon2id
    /// password hasher, token services and the Google token validator.
    /// Authentication schemes (JWT bearer, API tokens) are configured in the Api project.
    /// </summary>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        // Options, validated at startup so a missing secret fails fast with a clear message.
        services.AddOptions<JwtOptions>().Bind(config.GetSection(JwtOptions.Section)).ValidateDataAnnotations().ValidateOnStart();
        services.Configure<GoogleAuthOptions>(config.GetSection(GoogleAuthOptions.Section));
        services.Configure<AdminOptions>(config.GetSection(AdminOptions.Section));
        services.Configure<SeedOptions>(config.GetSection(SeedOptions.Section));

        var connection = config.GetConnectionString("Default")
            ?? throw new InvalidOperationException("Connection string 'Default' is missing. See api/README.md.");
        services.AddDbContext<AppDbContext>(o => o.UseNpgsql(connection));
        services.AddScoped<IAppDbContext>(sp => sp.GetRequiredService<AppDbContext>());

        // Identity without cookies or UI: we only need users, roles, logins and password checks.
        services.AddIdentityCore<AppUser>(o =>
            {
                o.User.RequireUniqueEmail = true;
                o.Password.RequiredLength = 12;
                o.Password.RequireNonAlphanumeric = false;
                o.Lockout.MaxFailedAccessAttempts = 5;
                o.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
            })
            .AddRoles<IdentityRole<Guid>>()
            .AddEntityFrameworkStores<AppDbContext>();
        services.AddScoped<IPasswordHasher<AppUser>, Argon2idPasswordHasher<AppUser>>();

        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUser, HttpCurrentUser>();
        services.AddSingleton<ITokenService, JwtTokenService>();
        services.AddSingleton<IGoogleTokenValidator, GoogleTokenValidator>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IApiTokenService, ApiTokenService>();

        // Image compression for design uploads. Stateless, so one shared instance is enough.
        services.AddSingleton<IImageCompressor, SkiaWebpCompressor>();

        return services;
    }
}
