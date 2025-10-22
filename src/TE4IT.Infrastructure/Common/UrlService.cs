using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using TE4IT.Application.Abstractions.Common;

namespace TE4IT.Infrastructure.Common;

/// <summary>
/// URL oluşturma ve yönetimi servisi
/// Environment-aware URL yönetimi sağlar
/// </summary>
public sealed class UrlService : IUrlService
{
    private readonly IConfiguration _configuration;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UrlService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
    {
        _configuration = configuration;
        _httpContextAccessor = httpContextAccessor;
    }

    public string GetFrontendUrl()
    {
        // 1. Önce configuration'dan frontend URL'i al
        var frontendUrl = _configuration["FrontendUrl"] ?? _configuration["FRONTEND_URL"];
        
        if (!string.IsNullOrEmpty(frontendUrl))
        {
            return frontendUrl;
        }

        // 2. Environment'a göre default URL döndür
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        
        if (environment == "Development")
        {
            // Development ortamında localhost kullan
            return "http://localhost:3000";
        }

        // 3. Production'da backend URL'den türet (fallback)
        return GetBackendUrl();
    }

    public string GetBackendUrl()
    {
        // 1. Önce configuration'dan backend URL'i al
        var backendUrl = _configuration["BackendUrl"] ?? _configuration["BACKEND_URL"];
        
        if (!string.IsNullOrEmpty(backendUrl))
        {
            return backendUrl;
        }

        // 2. HTTP Context'ten al (runtime URL)
        var context = _httpContextAccessor.HttpContext;
        if (context != null)
        {
            return $"{context.Request.Scheme}://{context.Request.Host}";
        }

        // 3. Environment'a göre default URL döndür
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        
        if (environment == "Development")
        {
            return "https://localhost:5001";
        }

        // Production default
        return "https://te4it-api.azurewebsites.net";
    }
}

