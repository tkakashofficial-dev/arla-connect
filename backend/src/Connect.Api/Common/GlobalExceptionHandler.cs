using Connect.Application.Common.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using FluentValidationException = FluentValidation.ValidationException;

namespace Connect.Api.Common;

/// <summary>
/// Translates exceptions into RFC-7807 ProblemDetails so every error response
/// has the same shape. Domain/validation errors are logged at Warning, anything
/// unexpected at Error (and the detail is hidden from the client).
/// </summary>
public class GlobalExceptionHandler(
    IProblemDetailsService problemDetailsService,
    ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var (status, title, detail, errors) = Map(exception);

        if (status >= StatusCodes.Status500InternalServerError)
            logger.LogError(exception, "Unhandled exception");
        else
            logger.LogWarning("Handled {Exception}: {Message}", exception.GetType().Name, exception.Message);

        httpContext.Response.StatusCode = status;

        var problemDetails = new ProblemDetails
        {
            Status = status,
            Title = title,
            Detail = detail,
            Instance = httpContext.Request.Path
        };
        if (errors is not null)
            problemDetails.Extensions["errors"] = errors;

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = problemDetails
        });
    }

    private static (int Status, string Title, string? Detail, IDictionary<string, string[]>? Errors) Map(
        Exception exception) => exception switch
    {
        FluentValidationException ve => (
            StatusCodes.Status400BadRequest,
            "Validation failed",
            null,
            ve.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray())),

        NotFoundException => (StatusCodes.Status404NotFound, "Resource not found", exception.Message, null),

        ConflictException => (StatusCodes.Status409Conflict, "Conflict", exception.Message, null),

        UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "Unauthorized", exception.Message, null),

        // Don't leak internal details to the client on unexpected errors.
        _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred", null, null)
    };
}
