using System.Net;
using System.Text.Json;

namespace Catalog.Api.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
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
            _logger.LogError(ex, "Unhandled exception for {Method} {Path}",
                context.Request.Method, context.Request.Path);

            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        var (status, title) = ex switch
        {
            KeyNotFoundException => (HttpStatusCode.NotFound, "Resource not found"),
            UnauthorizedAccessException => (HttpStatusCode.Forbidden, "Access denied"),
            ArgumentException => (HttpStatusCode.BadRequest, "Invalid argument"),
            InvalidOperationException => (HttpStatusCode.UnprocessableEntity, "Invalid operation"),
            _ => (HttpStatusCode.InternalServerError, "An unexpected error occurred")
        };

        var body = JsonSerializer.Serialize(new
        {
            title,
            status = (int)status,
            detail = ex.Message,
            traceId = context.TraceIdentifier
        });

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = (int)status;

        return context.Response.WriteAsync(body);
    }
}


