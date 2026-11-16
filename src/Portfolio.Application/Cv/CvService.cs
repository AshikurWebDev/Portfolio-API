using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Portfolio.Application.Common.Exceptions;
using Portfolio.Application.Common.Interfaces;
using Portfolio.Application.Cv.Dtos;
using Portfolio.Domain.Entities;

namespace Portfolio.Application.Cv;

/// <summary>Default <see cref="ICvService"/>: one PDF row in the database.</summary>
public sealed class CvService(IAppDbContext db, ISiteEvents events) : ICvService
{
    /// <summary>Largest CV accepted (5 MB). A text CV is usually well under 1 MB.</summary>
    public const int MaxBytes = 5 * 1024 * 1024;

    /// <summary>Public download path. The API serves it; the website links to it.</summary>
    public const string DownloadPath = "/api/cv";

    /// <inheritdoc />
    public async Task<CvFile?> GetFileAsync(CancellationToken ct) =>
        await db.CvDocuments
            .AsNoTracking()
            .OrderByDescending(c => c.UpdatedAt)
            .Select(c => new CvFile(c.Data, c.Version, c.UpdatedAt))
            .FirstOrDefaultAsync(ct);

    /// <inheritdoc />
    public async Task<CvInfoResponse?> GetInfoAsync(CancellationToken ct) =>
        // Only the length of the bytes is read here, never the bytes themselves.
        await db.CvDocuments
            .AsNoTracking()
            .OrderByDescending(c => c.UpdatedAt)
            .Select(c => new CvInfoResponse(c.OriginalFileName, c.Data.Length, c.UpdatedAt, DownloadPath + "?v=" + c.Version))
            .FirstOrDefaultAsync(ct);

    /// <inheritdoc />
    public async Task<CvInfoResponse> UploadAsync(Stream pdf, string fileName, CancellationToken ct)
    {
        var data = await ReadLimitedAsync(pdf, ct);

        // Check the content, not the file name or the browser's content type: every PDF starts with "%PDF-".
        if (data.Length < 5 || data[0] != '%' || data[1] != 'P' || data[2] != 'D' || data[3] != 'F' || data[4] != '-')
            throw new RequestValidationException("file", "Upload a PDF file.");

        var name = Path.GetFileName(fileName ?? string.Empty).Trim();
        if (name.Length is 0 or > 200) name = "cv.pdf";

        // Keep a single row: remove any earlier CV, then add the new one.
        await db.CvDocuments.ExecuteDeleteAsync(ct);
        db.CvDocuments.Add(new CvDocument
        {
            Id = Guid.CreateVersion7(),
            OriginalFileName = name,
            Data = data,
            Version = Convert.ToHexStringLower(SHA256.HashData(data))[..16],
        });
        await db.SaveChangesAsync(ct);
        await events.ContentChangedAsync(SiteTopics.Cv, ct);
        return (await GetInfoAsync(ct))!;
    }

    /// <inheritdoc />
    public async Task DeleteAsync(CancellationToken ct)
    {
        var deleted = await db.CvDocuments.ExecuteDeleteAsync(ct);
        if (deleted == 0) throw new NotFoundException("CV", "current");
        await events.ContentChangedAsync(SiteTopics.Cv, ct);
    }

    /// <summary>Reads at most <see cref="MaxBytes"/>; refuses larger files without reading them all.</summary>
    private static async Task<byte[]> ReadLimitedAsync(Stream input, CancellationToken ct)
    {
        using var buffer = new MemoryStream();
        var chunk = new byte[81920];
        int read;
        while ((read = await input.ReadAsync(chunk, ct)) > 0)
        {
            if (buffer.Length + read > MaxBytes)
                throw new RequestValidationException("file", "The CV must be 5 MB or smaller.");
            buffer.Write(chunk, 0, read);
        }
        return buffer.ToArray();
    }
}
