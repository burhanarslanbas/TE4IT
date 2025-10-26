using Microsoft.AspNetCore.RateLimiting;
using TE4IT.API.Configuration.Constants;
using TE4IT.API.Extensions;

namespace TE4IT.API.Configuration;

public static class ApiServiceConfiguration
{
    public static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddCorsConfiguration();
        services.AddSwaggerDocs();
        services.AddRateLimitingConfiguration();
        
        return services;
    }
    
    private static IServiceCollection AddCorsConfiguration(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
        options.AddPolicy("Frontend", policy => policy
            .WithOrigins(CorsOrigins.GetFrontendOrigins())
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
        );
        });
        
        return services;
    }
    
    private static IServiceCollection AddRateLimitingConfiguration(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            // ✅ Refresh token için rate limiting
            options.AddFixedWindowLimiter(RateLimitPolicies.FixedRefresh, opt =>
            {
                opt.Window = RateLimitPolicies.FixedRefreshPolicy.Window;
                opt.PermitLimit = RateLimitPolicies.FixedRefreshPolicy.PermitLimit;
                opt.QueueLimit = RateLimitPolicies.FixedRefreshPolicy.QueueLimit;
            });
            
            // ✅ Login endpoint'i için sıkı rate limiting
            options.AddFixedWindowLimiter(RateLimitPolicies.AuthPolicy, opt =>
            {
                opt.Window = RateLimitPolicies.AuthPolicyConfig.Window;
                opt.PermitLimit = RateLimitPolicies.AuthPolicyConfig.PermitLimit;
                opt.QueueLimit = RateLimitPolicies.AuthPolicyConfig.QueueLimit;
            });
            
            // ✅ Genel API için rate limiting
            options.AddFixedWindowLimiter(RateLimitPolicies.ApiPolicy, opt =>
            {
                opt.Window = RateLimitPolicies.ApiPolicyConfig.Window;
                opt.PermitLimit = RateLimitPolicies.ApiPolicyConfig.PermitLimit;
                opt.QueueLimit = RateLimitPolicies.ApiPolicyConfig.QueueLimit;
            });
            
            // ✅ Rate limit exceeded response
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
        });
        
        return services;
    }
}
