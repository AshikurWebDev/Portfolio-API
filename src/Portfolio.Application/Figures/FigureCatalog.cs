namespace Portfolio.Application.Figures;

/// <summary>
/// The figures the website knows how to draw. Keep this list in sync with
/// web/src/blog/figures/index.ts: a figure id that is not in both places will
/// not render on the site.
/// </summary>
public static class FigureCatalog
{
    /// <summary>All available figures.</summary>
    public static IReadOnlyList<FigureInfo> All { get; } =
    [
        new("zkteco-sync", "Flow: fingerprint terminal, sync worker, ERP API, database with a unique key on punches."),
        new("tenant-request", "Flow: request with tenant_id claim, tenant context, DbContext filter, only one tenant's rows returned (with sample table)."),
        new("tenant-stack", "3D stack of the four layers (Interface, API, Business logic, Data) split by organization, one organization lit through all layers."),
        new("db-choice", "Table comparing the coaching center ERP (SQL Server) and the land syndicate SaaS (PostgreSQL + PostGIS)."),
    ];

    /// <summary>True when <paramref name="id"/> is a known figure.</summary>
    public static bool Exists(string id) => All.Any(f => f.Id == id);
}
