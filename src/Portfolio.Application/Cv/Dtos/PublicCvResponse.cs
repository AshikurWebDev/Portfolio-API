namespace Portfolio.Application.Cv.Dtos;

/// <summary>What the public site needs to show (or hide) its "Download CV" buttons.</summary>
/// <param name="Available">True when a CV is uploaded.</param>
/// <param name="Url">Download URL (changes when a new CV is uploaded), or null.</param>
/// <param name="UploadedAt">When the CV was uploaded (UTC), or null.</param>
public sealed record PublicCvResponse(bool Available, string? Url, DateTimeOffset? UploadedAt);
