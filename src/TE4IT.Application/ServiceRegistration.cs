using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using TE4IT.Application.Behaviors;

namespace TE4IT.Application;

public static class ServiceRegistration
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // MediatR + Behaviors
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(ServiceRegistration).Assembly);
            
            // Validation behavior ekle (pipeline'a)
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        // FluentValidation - Assembly'deki tüm validator'ları bul ve register et
        services.AddValidatorsFromAssembly(typeof(ServiceRegistration).Assembly);

        return services;
    }
}


