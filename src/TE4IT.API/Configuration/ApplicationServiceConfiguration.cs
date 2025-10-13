using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TE4IT.Application;
using TE4IT.Infrastructure;
using TE4IT.Persistence.Relational.Extensions;

namespace TE4IT.API.Configuration;

public static class ApplicationServiceConfiguration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddApplication()
            .AddInfrastructure()
            .AddRelational(opt => opt.UseNpgsql(configuration.GetConnectionString("Pgsql")));
        
        return services;
    }
}
