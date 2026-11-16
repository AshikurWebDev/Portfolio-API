using Portfolio.Application.Common.Exceptions;
using Portfolio.Application.Common.Images;
using SkiaSharp;

namespace Portfolio.Infrastructure.Images;

/// <summary>
/// <see cref="IImageCompressor"/> built on SkiaSharp (MIT licence; the same engine Chrome uses).
/// <para>
/// Strategy, in order of how much it hurts the picture:
/// 1. cap the longest side at <see cref="MaxEdge"/> px (never upscale);
/// 2. lower WebP quality step by step (82 down to 45);
/// 3. if still above 100 KB, shrink both sides by the same factor and try again.
/// Width and height always change by the same factor, so the aspect ratio is kept.
/// </para>
/// </summary>
public sealed class SkiaWebpCompressor : IImageCompressor
{
    /// <summary>Longest side after compression. 1600 px is sharp in a gallery and on a phone.</summary>
    private const int MaxEdge = 1600;

    /// <summary>We stop shrinking here; below this the image would look poor.</summary>
    private const int MinEdge = 320;

    /// <summary>Refuse "decompression bombs": tiny files that expand to huge bitmaps (here 50 megapixels).</summary>
    private const long MaxInputPixels = 50_000_000;

    private static readonly int[] Qualities = [82, 74, 66, 58, 50, 45];

    /// <inheritdoc />
    public CompressedImage CompressToWebp(Stream input)
    {
        using var data = SKData.Create(input);
        using var codec = data is null ? null : SKCodec.Create(data);
        if (codec is null)
            throw new RequestValidationException("file", "This file is not an image we can read. Use JPEG, PNG, WebP or GIF.");

        if ((long)codec.Info.Width * codec.Info.Height > MaxInputPixels)
            throw new RequestValidationException("file", "This image is too large in pixels. Please export it at a smaller size.");

        // Decode, then turn it the right way up (phone photos store rotation in EXIF instead of rotating pixels).
        // Re-encoding below writes no metadata, so EXIF (camera, GPS location) is dropped as a side effect.
        using var decoded = SKBitmap.Decode(codec)
            ?? throw new RequestValidationException("file", "This image could not be decoded.");
        using var upright = ApplyOrigin(decoded, codec.EncodedOrigin);

        var longEdge = Math.Max(upright.Width, upright.Height);
        var scale = Math.Min(1.0, (double)MaxEdge / longEdge);
        byte[]? smallest = null;
        (int W, int H) smallestSize = default;

        while (true)
        {
            var w = Math.Max(1, (int)Math.Round(upright.Width * scale));
            var h = Math.Max(1, (int)Math.Round(upright.Height * scale));
            using var resized = scale < 1.0
                ? upright.Resize(new SKImageInfo(w, h), new SKSamplingOptions(SKCubicResampler.Mitchell))
                : upright.Copy();
            using var image = SKImage.FromBitmap(resized);

            foreach (var quality in Qualities)
            {
                using var encoded = image.Encode(SKEncodedImageFormat.Webp, quality);
                var bytes = encoded.ToArray();
                if (bytes.Length <= IImageCompressor.MaxBytes)
                    return new CompressedImage(bytes, w, h);
                if (smallest is null || bytes.Length < smallest.Length)
                {
                    smallest = bytes;
                    smallestSize = (w, h);
                }
            }

            // Still too big at the lowest quality: shrink by 20% and try again.
            if (Math.Max(w, h) * 0.8 < MinEdge)
                break;
            scale *= 0.8;
        }

        // Extremely detailed images (noise, dense text) may not fit even at MinEdge. Refuse rather than
        // store something above the limit.
        throw new RequestValidationException("file",
            $"This image could not be made smaller than 100 KB (best was {smallest!.Length / 1024} KB at {smallestSize.W}x{smallestSize.H}). Try a simpler or smaller image.");
    }

    /// <summary>
    /// Returns a bitmap rotated/flipped so that "up" is up, based on the EXIF orientation.
    /// Always returns a new bitmap the caller disposes.
    /// </summary>
    private static SKBitmap ApplyOrigin(SKBitmap src, SKEncodedOrigin origin)
    {
        // Origins 5-8 swap width and height (the picture is turned 90 degrees).
        var swap = origin is SKEncodedOrigin.LeftTop or SKEncodedOrigin.RightTop or SKEncodedOrigin.RightBottom or SKEncodedOrigin.LeftBottom;
        var result = new SKBitmap(swap ? src.Height : src.Width, swap ? src.Width : src.Height, src.ColorType, src.AlphaType);
        using var canvas = new SKCanvas(result);
        float w = src.Width, h = src.Height;

        switch (origin)
        {
            case SKEncodedOrigin.TopRight: canvas.Scale(-1, 1, w / 2, 0); break;                      // mirror
            case SKEncodedOrigin.BottomRight: canvas.RotateDegrees(180, w / 2, h / 2); break;         // upside down
            case SKEncodedOrigin.BottomLeft: canvas.Scale(1, -1, 0, h / 2); break;                    // flipped
            case SKEncodedOrigin.LeftTop: canvas.Scale(-1, 1, h / 2, 0); canvas.Translate(h, 0); canvas.RotateDegrees(90); break;
            case SKEncodedOrigin.RightTop: canvas.Translate(h, 0); canvas.RotateDegrees(90); break;   // turned left
            case SKEncodedOrigin.RightBottom: canvas.Scale(-1, 1, h / 2, 0); canvas.Translate(0, w); canvas.RotateDegrees(270); break;
            case SKEncodedOrigin.LeftBottom: canvas.Translate(0, w); canvas.RotateDegrees(270); break; // turned right
        }

        using var image = SKImage.FromBitmap(src);
        canvas.DrawImage(image, 0, 0, new SKSamplingOptions(SKFilterMode.Linear));
        canvas.Flush();
        return result;
    }
}
