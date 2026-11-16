namespace Portfolio.Domain.Common;

/// <summary>
/// Base class for entities that record when they were created and last changed.
/// The values are set automatically by the DbContext when changes are saved
/// (see <c>AppDbContext.SaveChangesAsync</c>), so services never set them by hand.
/// </summary>
public abstract class AuditableEntity
{
    /// <summary>When the row was first saved (UTC).</summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>When the row was last saved (UTC).</summary>
    public DateTimeOffset UpdatedAt { get; set; }
}
