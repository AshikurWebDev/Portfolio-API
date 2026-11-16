using System.Text.Json.Serialization;
using Microsoft.AspNetCore.HttpOverrides;
using Portfolio.Api.Extensions;
using Portfolio.Api.Mcp;
using Portfolio.Api.Middleware;
using Portfolio.Api.Realtime;
using Portfolio.Application;
using Portfolio.Application.Common.Interfaces;
using Portfolio.Infrastructure;
using Portfolio.Infrastructure.Persistence.Seed;
using Scalar.AspNetCore;

// ---------------------------------------------------------------------------
// Portfolio API: blog posts, projects, contact form, admin auth and an MCP
// server for AI agents. See api/README.md for setup and an architecture tour.
// ---------------------------------------------------------------------------
var builder = WebApplication.CreateBuilder(args);

// Layers: Application (use cases) and Infrastructure (database, Identity, tokens).
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// Controllers (API only, no views). Enums travel as camelCase strings: "ui", "draft".
builder.Services.AddControllers()
    .AddJsonOptions(o =>
    {
        o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(System.Text.Json.JsonNamingPolicy.CamelCase));
        o.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

builder.Services.AddPortfolioAuth(builder.Configuration);
builder.Services.AddPortfolioRateLimits();

// Errors as RFC 7807 problem details.
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ApiExceptionHandler>();

// OpenAPI document at /openapi/v1.json, browsable docs at /scalar (Development only).
builder.Services.AddOpenApi();

// The website calls the API from another origin in production (e.g. https://ashikur.dev -> https://api.ashikur.dev).
var origins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
builder.Services.AddCors(o => o.AddDefaultPolicy(p => p.WithOrigins(origins).AllowAnyHeader().AllowAnyMethod().AllowCredentials()));

// Live updates: open pages hear "this content changed" over SignalR and refetch it (see Realtime/).
builder.Services.AddSignalR();
builder.Services.AddSingleton<ISiteEvents, SignalRSiteEvents>();

// MCP server for AI agents (Streamable HTTP transport, stateless: each call stands alone).
builder.Services.AddMcpServer(o => o.ServerInfo = new() { Name = "ashikur-portfolio-blog", Version = "1.0.0" })
    .WithHttpTransport(o => o.Stateless = true)
    .WithTools<BlogTools>()
    .WithPrompts<BlogPrompts>();

// In production the API sits behind a reverse proxy (Caddy, see deploy/). The proxy ends HTTPS and
// forwards plain HTTP, so without this the API would think every request is http:// from the proxy's IP:
// the rate limiter would put all visitors in one bucket. These headers carry the real scheme and client IP.
// Only the proxy can reach the API (it is not published on the host), so trusting any sender is safe here.
builder.Services.Configure<ForwardedHeadersOptions>(o =>
{
    o.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    o.KnownIPNetworks.Clear();
    o.KnownProxies.Clear();
});

var app = builder.Build();

app.UseForwardedHeaders();

// Applies migrations and creates the Admin role, projects and draft posts (each only when missing, see DbSeeder).
// Always in Development; in production when Database:MigrateOnStartup is true (set in deploy/.env).
// Google sign-in needs the Admin role to exist, so a fresh production database must run this once.
if (app.Environment.IsDevelopment() || app.Configuration.GetValue<bool>("Database:MigrateOnStartup"))
{
    await DbSeeder.SeedAsync(app.Services);
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseExceptionHandler();
app.UseStatusCodePages();
app.UseCors();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<SiteHub>(SiteHub.Path);
app.MapGet("/health", () => Results.Ok(new { status = "ok" })).ExcludeFromDescription();

// Only the admin (JWT or personal access token) may use the MCP server.
app.MapMcp("/mcp").RequireAuthorization(AuthenticationSetup.AdminPolicy);

app.Run();

/// <summary>Entry point; public so integration tests can start the app with WebApplicationFactory.</summary>
public partial class Program;
