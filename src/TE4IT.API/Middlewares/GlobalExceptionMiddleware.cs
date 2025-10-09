using System.Net;
using System.Text.Json;
using FluentValidation;
using TE4IT.Domain.Exceptions;

namespace TE4IT.API.Middlewares;

/// <summary>
/// Global exception handler middleware
/// Tüm unhandled exception'ları yakalar ve uygun HTTP response döner
/// </summary>
public class GlobalExceptionMiddleware(
    RequestDelegate next,
    ILogger<GlobalExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception occurred: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var (statusCode, errors) = exception switch
        {
            // FluentValidation hatası
            ValidationException validationEx => (
                HttpStatusCode.BadRequest,
                validationEx.Errors.Select(e => new
                {
                    field = e.PropertyName,
                    message = e.ErrorMessage
                }).ToList<object>()
            ),

                // Özel Domain Exceptions (önce özel tipler!)
                ResourceNotFoundException => (
                    HttpStatusCode.NotFound,
                    new List<object> { new { message = exception.Message } }
                ),

                InvalidTaskStateTransitionException => (
                    HttpStatusCode.BadRequest,
                    new List<object> { new { message = exception.Message } }
                ),

                TaskDependencyViolationException => (
                    HttpStatusCode.BadRequest,
                    new List<object> { new { message = exception.Message } }
                ),

                BusinessRuleViolationException => (
                    HttpStatusCode.BadRequest,
                    new List<object> { new { message = exception.Message } }
                ),

                // Genel Domain Exception (en sona!)
                DomainException domainEx => (
                    HttpStatusCode.BadRequest,
                    new List<object> { new { message = domainEx.Message } }
                ),

            // Auth exceptions
            UnauthorizedAccessException => (
                HttpStatusCode.Unauthorized,
                new List<object> { new { message = "Unauthorized access" } }
            ),

            // Default - Internal Server Error
            _ => (
                HttpStatusCode.InternalServerError,
                new List<object> { new { message = "An unexpected error occurred. Please try again later." } }
            )
        };

        context.Response.StatusCode = (int)statusCode;

        var response = new
        {
            statusCode = (int)statusCode,
            errors,
            timestamp = DateTime.UtcNow,
            path = context.Request.Path.ToString()
        };

        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(json);
    }
}

