namespace Portfolio.Application.Common.Exceptions;

/// <summary>Thrown when a requested record does not exist. Mapped to HTTP 404 by the API.</summary>
public sealed class NotFoundException(string resource, object key)
    : Exception($"{resource} '{key}' was not found.")
{
    /// <summary>Name of the missing resource, e.g. "Post".</summary>
    public string Resource { get; } = resource;
}
