using Microsoft.AspNetCore.RateLimiting;
using TE4IT.API.Extensions;
using TE4IT.API.Middlewares;

namespace TE4IT.API.Configuration;

public static class MiddlewareConfiguration
{
    public static WebApplication ConfigureMiddlewarePipeline(this WebApplication app)
    {
        // Core middleware'ler - en önce çalışmalı
        app.UseCoreMiddleware();
        
        // Environment-specific middleware'ler
        app.UseEnvironmentSpecificMiddleware();
        
        // Security middleware'ler
        app.UseSecurityMiddleware();
        
        // API middleware'ler
        app.UseApiMiddleware();
        
        return app;
    }
    
    private static WebApplication UseCoreMiddleware(this WebApplication app)
    {
        // Global exception handling - tüm hataları yakalar, en önce olmalı
        app.UseMiddleware<GlobalExceptionMiddleware>();
        
        // HTTPS redirection
        app.UseHttpsRedirection();
        
        return app;
    }
    
    private static WebApplication UseEnvironmentSpecificMiddleware(this WebApplication app)
    {
        // Swagger'ı hem Development hem de Production'da aç
        app.UseSwaggerWithUI(redirectRoot: true);
        
        return app;
    }
    
    private static WebApplication UseSecurityMiddleware(this WebApplication app)
    {
        // CORS - authentication'dan önce olmalı
        app.UseCors("Frontend");
        
        // Authentication ve Authorization
        app.UseAuthentication();
        app.UseRateLimiter(new RateLimiterOptions());
        app.UseAuthorization();
        
        return app;
    }
    
    private static WebApplication UseApiMiddleware(this WebApplication app)
    {
        // API endpoints
        app.MapControllers();
        
        return app;
    }
}
