namespace Portfolio.Application.Cv.Dtos;

/// <summary>The stored CV for the download endpoint.</summary>
/// <param name="Data">PDF bytes.</param>
/// <param name="Version">Content hash, used as the ETag.</param>
/// <param name="UploadedAt">When it was uploaded, used as Last-Modified.</param>
public sealed record CvFile(byte[] Data, string Version, DateTimeOffset UploadedAt);
