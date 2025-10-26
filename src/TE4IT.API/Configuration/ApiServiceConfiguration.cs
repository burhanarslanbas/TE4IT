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
