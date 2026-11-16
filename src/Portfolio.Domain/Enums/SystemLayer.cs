namespace Portfolio.Domain.Enums;

/// <summary>
/// The four layers of a system, as shown in the "How I build" section of the site.
/// Every blog post is filed under one of them. Serialized to JSON as
/// "ui", "api", "logic" and "data" (camelCase string enum).
/// </summary>
public enum SystemLayer
{
    /// <summary>Interface: React + TypeScript screens.</summary>
    Ui,

    /// <summary>API: ASP.NET Core Web API.</summary>
    Api,

    /// <summary>Business logic: C# services.</summary>
    Logic,

    /// <summary>Data: SQL Server / PostgreSQL.</summary>
    Data,
}
