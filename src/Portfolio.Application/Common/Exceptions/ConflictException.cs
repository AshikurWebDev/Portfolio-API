namespace Portfolio.Application.Common.Exceptions;

/// <summary>Thrown when a change clashes with existing data (e.g. a duplicate slug). Mapped to HTTP 409.</summary>
public sealed class ConflictException(string message) : Exception(message);
