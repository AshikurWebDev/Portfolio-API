using System.ComponentModel.DataAnnotations;

namespace Portfolio.Application.Design.Dtos;

/// <summary>Payload for changing a design item's text and visibility (the image is replaced separately).</summary>
public sealed record UpdateDesignPostRequest
{
    /// <summary>Short description; also the image's alt text.</summary>
    [Required, MaxLength(200)]
    public required string Caption { get; init; }

    /// <summary>Optional http(s) link to the original post.</summary>
    [Url, MaxLength(500)]
    public string? Link { get; init; }

    /// <summary>Show the item on the site.</summary>
    public bool IsVisible { get; init; } = true;
}
