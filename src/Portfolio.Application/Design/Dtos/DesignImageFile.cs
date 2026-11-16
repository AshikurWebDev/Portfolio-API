namespace Portfolio.Application.Design.Dtos;

/// <summary>A stored design image, ready to send as a file.</summary>
/// <param name="Data">WebP bytes.</param>
/// <param name="Version">Content hash, used as the HTTP ETag.</param>
public sealed record DesignImageFile(byte[] Data, string Version);
