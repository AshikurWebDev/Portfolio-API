using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

namespace Portfolio.Api.Extensions;

/// <summary>Rate limits for endpoints anyone can call. Exceeding a limit returns 429.</summary>
public static class RateLimitSetup
{
    /// <summary>Contact form: 5 messages per 10 minutes per IP address.</summary>
    public const string Contact = "contact";

    /// <summary>Sign-in: 10 attempts per minute per IP address.</summary>
    public const string Auth = "auth";

    /// <summary>Registers the rate limiter policies.</summary>
    public static IServiceCollection AddPortfolioRateLimits(this IServiceCollection services)
    {
        services.AddRateLimiter(o =>
        {
            o.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            o.AddPolicy(Contact, ctx => RateLimitPartition.GetFixedWindowLimiter(
                ctx.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                _ => new FixedWindowRateLimiterOptions { PermitLimit = 5, Window = TimeSpan.FromMinutes(10) }));
            o.AddPolicy(Auth, ctx => RateLimitPartition.GetFixedWindowLimiter(
                ctx.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                _ => new FixedWindowRateLimiterOptions { PermitLimit = 10, Window = TimeSpan.FromMinutes(1) }));
        });
        return services;
    }
}
