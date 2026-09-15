using System.Net;
using System.Text.Json;
using FinanceApi.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace FinanceApi.Middleware;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception. TraceId: {TraceId}", context.TraceIdentifier);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, title) = exception switch
        {
            NotFoundException => (HttpStatusCode.NotFound, "Resource not found"),
            ValidationException => (HttpStatusCode.BadRequest, "Validation failed"),
            KeyNotFoundException => (HttpStatusCode.NotFound, "Resource not found"),
            ArgumentException => (HttpStatusCode.BadRequest, "Invalid argument"),
            NotImplementedException => (HttpStatusCode.NotImplemented, "Not implemented"),
            _ => (HttpStatusCode.InternalServerError, "An unexpected error occurred")
        };

        var problemDetails = new ProblemDetails
        {
            Status = (int)statusCode,
            Title = title,
            Detail = exception.Message,
            Instance = context.Request.Path,
            Type = $"https://httpstatuses.io/{(int)statusCode}"
        };

        problemDetails.Extensions["traceId"] = context.TraceIdentifier;

        if (exception is ValidationException { Errors: not null } validationException)
        {
            problemDetails.Extensions["errors"] = validationException.Errors;
        }

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = (int)statusCode;

        return context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails));
    }
}
