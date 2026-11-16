namespace Portfolio.Application.Common.Exceptions;

/// <summary>Thrown when the caller is known but not allowed to do this (e.g. a Google account not on the admin list). Mapped to HTTP 403.</summary>
public sealed class ForbiddenException(string message) : Exception(message);
