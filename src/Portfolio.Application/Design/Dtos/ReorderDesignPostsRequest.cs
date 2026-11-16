using System.ComponentModel.DataAnnotations;

namespace Portfolio.Application.Design.Dtos;

/// <summary>The full gallery order: every item id, first item first.</summary>
public sealed record ReorderDesignPostsRequest
{
    /// <summary>All design item ids in the new order.</summary>
    [Required, MinLength(1), MaxLength(500)]
    public required IReadOnlyList<Guid> Ids { get; init; }
}
