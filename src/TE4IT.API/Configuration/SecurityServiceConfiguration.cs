using Microsoft.Extensions.Configuration;
using TE4IT.Infrastructure;

namespace TE4IT.API.Configuration;

public static class SecurityServiceConfiguration
{
    public static IServiceCollection AddSecurityServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddIdentityAndJwtAuthentication(configuration);
        return services;
    }
}
