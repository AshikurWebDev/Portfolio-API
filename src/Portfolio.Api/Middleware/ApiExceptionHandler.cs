using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Application.Common.Exceptions;

namespace Portfolio.Api.Middleware;

/// <summary>
/// Turns the application's exceptions into RFC 7807 problem details responses,
/// so controllers need no try/catch. Unknown exceptions fall through to the
/// default handler (500, details hidden outside Development).
/// </summary>
internal sealed class ApiExceptionHandler(ILogger<ApiExceptionHandler> logger, IProblemDetailsService problems)
    : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext http, Exception exception, CancellationToken ct)
    {
        if (exception is RequestValidationException validation)
        {
            logger.LogInformation("Validation failed: {Message}", validation.Message);
            http.Response.StatusCode = StatusCodes.Status400BadRequest;
            return await problems.TryWriteAsync(new ProblemDetailsContext
            {
                HttpContext = http,
                ProblemDetails = new ValidationProblemDetails(validation.Errors)
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "One or more fields are invalid.",
                },
            });
        }

        // These messages are written for the user and contain no internals, so they are safe to return.
        var (status, title) = exception switch
        {
            NotFoundException => (StatusCodes.Status404NotFound, "Not found"),
            ConflictException => (StatusCodes.Status409Conflict, "Conflict"),
            UnauthorizedException => (StatusCodes.Status401Unauthorized, "Unauthorized"),
            ForbiddenException => (StatusCodes.Status403Forbidden, "Forbidden"),
            _ => (0, string.Empty),
        };
        if (status == 0) return false;

        logger.LogInformation("Handled {Exception}: {Message}", exception.GetType().Name, exception.Message);
        http.Response.StatusCode = status;
        return await problems.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = http,
            ProblemDetails = new ProblemDetails { Status = status, Title = title, Detail = exception.Message },
        });
    }
}
