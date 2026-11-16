namespace Portfolio.Application.Common.Exceptions;

/// <summary>Thrown when credentials are missing or wrong. Mapped to HTTP 401.</summary>
public sealed class UnauthorizedException(string message) : Exception(message);
