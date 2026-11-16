namespace Portfolio.Application.Posts.Dtos;

/// <summary>A post's cover image.</summary>
/// <param name="Src">Image URL.</param>
/// <param name="Alt">Alternative text.</param>
public sealed record CoverResponse(string Src, string Alt);
