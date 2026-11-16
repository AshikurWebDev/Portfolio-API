namespace Portfolio.Application.Common.Images;

/// <summary>
/// Turns an uploaded image (JPEG, PNG, WebP, GIF, BMP) into a small WebP.
/// Defined here, implemented in Infrastructure (it needs an imaging library).
/// </summary>
public interface IImageCompressor
{
    /// <summary>Upper size limit for the output, in bytes (100 KB).</summary>
    const int MaxBytes = 100_000;

    /// <summary>
    /// Decodes the image, applies its camera rotation (EXIF), drops metadata, keeps the
    /// aspect ratio, and lowers quality and then size until the WebP fits in <see cref="MaxBytes"/>.
    /// Throws <see cref="Exceptions.RequestValidationException"/> when the input is not a readable image
    /// or is unreasonably large in pixels.
    /// </summary>
    CompressedImage CompressToWebp(Stream input);
}
