namespace Portfolio.Application.Common.Images;

/// <summary>Result of <see cref="IImageCompressor.CompressToWebp"/>.</summary>
/// <param name="Data">WebP bytes, never larger than <see cref="IImageCompressor.MaxBytes"/>.</param>
/// <param name="Width">Width in pixels.</param>
/// <param name="Height">Height in pixels.</param>
public sealed record CompressedImage(byte[] Data, int Width, int Height);
