using System.Net;
using System.Text.Json;
using FluentValidation;
using TE4IT.Domain.Exceptions.Base;
using TE4IT.Domain.Exceptions.Auth;
using TE4IT.Domain.Exceptions.Common;
using TE4IT.Domain.Exceptions.Tasks;
using TE4IT.Domain.Exceptions.Projects;

namespace TE4IT.API.Middlewares;

/// <summary>
/// Global Exception Handler Middleware
/// 
/// Bu middleware'in görevi:
/// 1. Uygulamada fırlatılan tüm exception'ları yakalamak
/// 2. Her exception türü için uygun HTTP response döndürmek
/// 3. Kullanıcıya anlamlı hata mesajları vermek
/// </summary>
public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    /// <summary>
    /// Constructor - Dependency Injection ile gelen servisleri alır
    /// </summary>
    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next; // Sonraki middleware'i temsil eder
        _logger = logger; // Loglama için kullanılır
    }

    /// <summary>
    /// Middleware'in ana metodu - her HTTP request'te çalışır
    /// </summary>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            // Sonraki middleware'i çağır (Controller'a git)
            await _next(context);
        }
        catch (Exception ex)
        {
            // Herhangi bir exception fırlatılırsa buraya gelir
            _logger.LogError(ex, "Unhandled exception occurred: {Message}", ex.Message);
            
            // Exception'ı handle et ve HTTP response döndür
            await HandleExceptionAsync(context, ex);
        }
    }

    /// <summary>
    /// Exception'ı yakalar ve uygun HTTP response'a çevirir
    /// </summary>
    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        // Response'ın JSON formatında olacağını belirt
        context.Response.ContentType = "application/json";

        // Exception türüne göre HTTP status code ve error message belirle
        var (statusCode, errorResponse) = DetermineErrorResponse(exception);

        // HTTP status code'u ayarla
        context.Response.StatusCode = (int)statusCode;

        // Response body'sini oluştur
        var response = new
        {
            statusCode = (int)statusCode,
            message = errorResponse,
            timestamp = DateTime.UtcNow,
            path = context.Request.Path.ToString()
        };

        // Response'ı JSON'a çevir ve gönder
        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase // camelCase formatında
        });

        await context.Response.WriteAsync(json);
    }

    /// <summary>
    /// Exception türüne göre uygun HTTP status code ve error message belirler
    /// Bu metod switch expression kullanarak exception'ları kategorize eder
    /// </summary>
    private static (HttpStatusCode statusCode, object errorResponse) DetermineErrorResponse(Exception exception)
    {
        return exception switch
        {
            // 1. VALIDATION HATALARI (400 Bad Request)
            // FluentValidation'dan gelen hatalar
            ValidationException validationEx => (
                HttpStatusCode.BadRequest,
                new
                {
                    message = "Validation failed",
                    errors = validationEx.Errors.Select(e => new
                    {
                        field = e.PropertyName,
                        message = e.ErrorMessage
                    }).ToList()
                }
            ),

            // 2. AUTH HATALARI
            // Kullanıcı adı zaten kullanımda
            DuplicateUserNameException duplicateUserNameEx => (
                HttpStatusCode.BadRequest,
                new
                {
                    message = duplicateUserNameEx.Message,
                    field = "userName"
                }
            ),

            // E-posta zaten kullanımda
            DuplicateEmailException duplicateEmailEx => (
                HttpStatusCode.BadRequest,
                new
                {
                    message = duplicateEmailEx.Message,
                    field = "email"
                }
            ),

            // Kullanıcı kaydı başarısız
            UserRegistrationFailedException registrationFailedEx => (
                HttpStatusCode.BadRequest,
                new { message = registrationFailedEx.Message }
            ),

            // Yanlış giriş bilgileri
            InvalidCredentialsException invalidCredentialsEx => (
                HttpStatusCode.Unauthorized,
                new { message = invalidCredentialsEx.Message }
            ),

            // 3. DOMAIN HATALARI (Business Logic)
            // Kaynak bulunamadı
            ResourceNotFoundException => (
                HttpStatusCode.NotFound,
                new { message = exception.Message }
            ),

            // Geçersiz task durumu geçişi
            InvalidTaskStateTransitionException => (
                HttpStatusCode.BadRequest,
                new { message = exception.Message }
            ),

            // Task bağımlılık ihlali
            TaskDependencyViolationException => (
                HttpStatusCode.BadRequest,
                new { message = exception.Message }
            ),

            // İş kuralı ihlali
            BusinessRuleViolationException => (
                HttpStatusCode.BadRequest,
                new { message = exception.Message }
            ),

            // 4. YETKİ HATALARI (401 Unauthorized)
            UnauthorizedAccessException => (
                HttpStatusCode.Unauthorized,
                new { message = "Unauthorized access" }
            ),

            // 5. DEFAULT - Bilinmeyen hatalar (500 Internal Server Error)
            _ => (
                HttpStatusCode.InternalServerError,
                new { message = "An unexpected error occurred. Please try again later." }
            )
        };
    }
}