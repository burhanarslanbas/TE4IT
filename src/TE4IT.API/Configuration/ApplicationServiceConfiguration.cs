using Microsoft.EntityFrameworkCore;
using TE4IT.Application;
using TE4IT.Infrastructure;
using TE4IT.Persistence.TaskManagement.ServiceRegistrations;

namespace TE4IT.API.Configuration;

public static class ApplicationServiceConfiguration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddApplication()
            .AddInfrastructure(configuration)
            .AddRelational(opt => opt.UseNpgsql(configuration.GetConnectionString("Pgsql")));

        return services;
    }
}
