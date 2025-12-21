using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using TE4IT.Application;
using TE4IT.Infrastructure;
using TE4IT.Persistence.EducationManagement.DependencyInjection;
using TE4IT.Persistence.TaskManagement.ServiceRegistrations;

namespace TE4IT.API.Configuration;

public static class ApplicationServiceConfiguration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment? environment = null)
    {
        services
            .AddApplication()
            .AddInfrastructure(configuration);

        // Test ortamında AddRelational'ı atla (test base class'ında In-Memory ekleniyor)
        var envName = environment?.EnvironmentName ?? 
                     configuration["ASPNETCORE_ENVIRONMENT"] ?? 
                     Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? 
                     "Development";
        
        if (envName != "Testing")
        {
            services.AddRelational(opt => 
            {
                // Azure App Service için environment variable'dan connection string oku
                var connectionString = configuration.GetConnectionString("Pgsql") ?? 
                                    configuration["CONNECTION_STRING"];
                opt.UseNpgsql(connectionString);
            });

            // Education modülü için MongoDB persistence kaydı
            services.AddEducationMongo(configuration);
        }

        return services;
    }
}
