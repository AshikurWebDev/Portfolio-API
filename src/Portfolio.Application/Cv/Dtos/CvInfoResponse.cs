namespace Portfolio.Application.Cv.Dtos;

/// <summary>What the admin sees about the uploaded CV.</summary>
/// <param name="FileName">Name of the file that was uploaded.</param>
/// <param name="SizeBytes">PDF size in bytes.</param>
/// <param name="UploadedAt">When it was uploaded (UTC).</param>
/// <param name="Url">Public download URL.</param>
public sealed record CvInfoResponse(string FileName, long SizeBytes, DateTimeOffset UploadedAt, string Url);
