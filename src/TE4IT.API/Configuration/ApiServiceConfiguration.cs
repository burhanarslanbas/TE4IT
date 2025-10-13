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
                .WithOrigins(CorsOrigins.FrontendOrigins)
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
            options.AddFixedWindowLimiter(RateLimitPolicies.FixedRefresh, opt =>
            {
                opt.Window = RateLimitPolicies.FixedRefreshPolicy.Window;
                opt.PermitLimit = RateLimitPolicies.FixedRefreshPolicy.PermitLimit;
                opt.QueueLimit = RateLimitPolicies.FixedRefreshPolicy.QueueLimit;
            });
        });
        
        return services;
    }
}
