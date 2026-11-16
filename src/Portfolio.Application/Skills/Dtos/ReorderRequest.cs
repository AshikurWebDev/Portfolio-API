using System.ComponentModel.DataAnnotations;

namespace Portfolio.Application.Skills.Dtos;

/// <summary>A full order: every id exactly once, first item first.</summary>
public sealed record ReorderRequest
{
    /// <summary>All ids in the new order.</summary>
    [Required, MinLength(1), MaxLength(200)]
    public required IReadOnlyList<Guid> Ids { get; init; }
}
