using Portfolio.Application.Design.Dtos;

namespace Portfolio.Application.Design;

/// <summary>
/// The design gallery: public reads for the home page and admin management
/// (upload, edit, reorder, hide, delete). Images are compressed to WebP on upload.
/// </summary>
public interface IDesignPostService
{
    /// <summary>Visible items in gallery order, for the public site.</summary>
    Task<IReadOnlyList<DesignPostResponse>> GetVisibleAsync(CancellationToken ct);

    /// <summary>The stored WebP for one item, or null when it does not exist.</summary>
    Task<DesignImageFile?> GetImageAsync(Guid id, CancellationToken ct);

    /// <summary>All items including hidden ones, in gallery order, for the admin.</summary>
    Task<IReadOnlyList<AdminDesignPostResponse>> GetAllAsync(CancellationToken ct);

    /// <summary>One item for the admin. Throws NotFoundException when missing.</summary>
    Task<AdminDesignPostResponse> GetAsync(Guid id, CancellationToken ct);

    /// <summary>Compresses the image and adds a new item at the end of the gallery.</summary>
    Task<AdminDesignPostResponse> CreateAsync(Stream image, string caption, string? link, CancellationToken ct);

    /// <summary>Changes caption, link and visibility.</summary>
    Task<AdminDesignPostResponse> UpdateAsync(Guid id, UpdateDesignPostRequest request, CancellationToken ct);

    /// <summary>Compresses a new image and swaps it in, keeping caption and position.</summary>
    Task<AdminDesignPostResponse> ReplaceImageAsync(Guid id, Stream image, CancellationToken ct);

    /// <summary>Saves a new order. The list must contain every item id exactly once.</summary>
    Task ReorderAsync(ReorderDesignPostsRequest request, CancellationToken ct);

    /// <summary>Deletes an item and its image.</summary>
    Task DeleteAsync(Guid id, CancellationToken ct);
}
