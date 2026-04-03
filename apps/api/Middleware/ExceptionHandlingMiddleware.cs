using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Middleware;

/// <summary>
/// Safety-net middleware for unhandled exceptions.
/// With the Result pattern in place, only unexpected failures should reach here:
/// - Domain guard clause violations (ArgumentException from entity constructors)
/// - FluentValidation failures from non-Result handlers (backward-compat)
/// - Infrastructure failures (DB, network)
/// </summary>
public sealed class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/problem+json";

        var (statusCode, title, detail, errors) = exception switch
        {
            ValidationException validationException => (
                StatusCodes.Status400BadRequest,
                "Validation Error",
                "One or more validation failures occurred.",
                (object?)validationException.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(x => x.ErrorMessage).ToArray())
            ),
            ArgumentException argException => (
                StatusCodes.Status422UnprocessableEntity,
                "Domain Rule Violation",
                argException.Message,
                (object?)null
            ),
            _ => (
                StatusCodes.Status500InternalServerError,
                "Internal Server Error",
                "An unexpected error occurred while processing your request.",
                (object?)null
            )
        };

        context.Response.StatusCode = statusCode;

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = context.Request.Path
        };

        if (errors != null)
            problemDetails.Extensions["errors"] = errors;

        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        await context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails, options));
    }
}
