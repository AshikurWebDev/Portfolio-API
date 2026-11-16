namespace Portfolio.Application.Common.Exceptions;

/// <summary>
/// Thrown when input breaks a business rule that data annotations cannot express.
/// Mapped to HTTP 400 with a validation problem details body.
/// </summary>
public sealed class RequestValidationException(string field, string error)
    : Exception(error)
{
    /// <summary>Field name to error messages, in the shape of ASP.NET Core's ValidationProblemDetails.</summary>
    public IDictionary<string, string[]> Errors { get; } = new Dictionary<string, string[]> { [field] = [error] };
}
