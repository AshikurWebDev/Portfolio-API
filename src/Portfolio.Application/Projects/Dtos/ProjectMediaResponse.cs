namespace Portfolio.Application.Projects.Dtos;

/// <summary>
/// Media on a project card, in the website's <c>Media</c> union shape:
/// image {type, src, alt}, video {type, src, poster, alt} or fragment {type, id}.
/// Unused fields are omitted from the JSON.
/// </summary>
/// <param name="Type">"image", "video" or "fragment".</param>
/// <param name="Src">Image or video URL.</param>
/// <param name="Poster">Video poster URL.</param>
/// <param name="Alt">Alternative text.</param>
/// <param name="Id">Fragment id.</param>
public sealed record ProjectMediaResponse(string Type, string? Src, string? Poster, string? Alt, string? Id);
