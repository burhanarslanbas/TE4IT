using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TE4IT.Abstractions.Persistence.Repositories.Modules;
using TE4IT.Abstractions.Persistence.Repositories.Projects;
using TE4IT.Abstractions.Persistence.Repositories.Tasks;
using TE4IT.Abstractions.Persistence.Repositories.UseCases;
using TE4IT.Application.Abstractions.Persistence;
using TE4IT.Persistence.Relational.Db;
using TE4IT.Persistence.Relational.Repositories.Modules;
using TE4IT.Persistence.Relational.Repositories.Projects;
using TE4IT.Persistence.Relational.Repositories.Tasks;
using TE4IT.Persistence.Relational.Repositories.UseCases;

namespace TE4IT.Persistence.Relational.Extensions;

public static class RelationalServiceRegistration
{
    public static IServiceCollection AddRelational(this IServiceCollection services, Action<DbContextOptionsBuilder> configureDb)
    {
        services.AddDbContext<AppDbContext>(configureDb);
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Register Read Repositories
        services.AddScoped<IModuleReadRepository, ModuleReadRepository>();
        services.AddScoped<IProjectReadRepository, ProjectReadRepository>();
        services.AddScoped<ITaskReadRepository, TaskReadRepository>();
        services.AddScoped<IUseCaseReadRepository, UseCaseReadRepository>();

        // Register Write Repositories
        services.AddScoped<IModuleWriteRepository, ModuleWriteRepository>();
        services.AddScoped<IProjectWriteRepository, ProjectWriteRepository>();
        services.AddScoped<ITaskWriteRepository, TaskWriteRepository>();
        services.AddScoped<IUseCaseWriteRepository, UseCaseWriteRepository>();

        return services;
    }
}