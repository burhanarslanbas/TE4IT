using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using TE4IT.API.Configuration.Constants;
using TE4IT.API.Extensions;

namespace TE4IT.API.Configuration;

public static class ApiServiceConfiguration
{
    public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration? configuration = null)
    {
        services.AddControllers();
        services.AddCorsConfiguration();
        services.AddSwaggerDocs();
        
        // Application Insights - Azure'da otomatik olarak APPINSIGHTS_INSTRUMENTATIONKEY veya 
        // APPLICATIONINSIGHTS_CONNECTION_STRING environment variable'ından okunur
        if (configuration != null)
        {
            var connectionString = configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"] 
                                ?? configuration["ApplicationInsights:ConnectionString"];
            
            if (!string.IsNullOrWhiteSpace(connectionString))
            {
                services.AddApplicationInsightsTelemetry(options =>
                {
                    options.ConnectionString = connectionString;
                    options.EnableAdaptiveSampling = true;
                    options.EnablePerformanceCounterCollectionModule = true;
                    options.EnableRequestTrackingTelemetryModule = true;
                });
            }
        }
        
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
}
