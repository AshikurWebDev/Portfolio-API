using Portfolio.Application.Cv.Dtos;

namespace Portfolio.Application.Cv;

/// <summary>The single CV PDF behind every "Download CV" button.</summary>
public interface ICvService
{
    /// <summary>The PDF for download, or null when none has been uploaded.</summary>
    Task<CvFile?> GetFileAsync(CancellationToken ct);

    /// <summary>Details for the admin, or null when none has been uploaded.</summary>
    Task<CvInfoResponse?> GetInfoAsync(CancellationToken ct);

    /// <summary>Checks that the stream is a PDF within the size limit and stores it, replacing any earlier CV.</summary>
    Task<CvInfoResponse> UploadAsync(Stream pdf, string fileName, CancellationToken ct);

    /// <summary>Removes the CV. The download buttons then report that no CV is available.</summary>
    Task DeleteAsync(CancellationToken ct);
}
