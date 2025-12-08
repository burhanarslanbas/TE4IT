using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TE4IT.Abstractions.Persistence.Repositories.Modules;
using TE4IT.Abstractions.Persistence.Repositories.Projects;
using TE4IT.Abstractions.Persistence.Repositories.Tasks;
using TE4IT.Abstractions.Persistence.Repositories.UseCases;
using TE4IT.Application.Abstractions.Persistence;
using TE4IT.Application.Abstractions.Persistence.Repositories.ProjectInvitations;
using TE4IT.Application.Abstractions.Persistence.Repositories.TaskRelations;
using TE4IT.Persistence.Common.Contexts;
using TE4IT.Persistence.Common.UnitOfWork;
using TE4IT.Persistence.TaskManagement.Repositories.Modules;
using TE4IT.Persistence.TaskManagement.Repositories.ProjectInvitations;
using TE4IT.Persistence.TaskManagement.Repositories.Projects;
using TE4IT.Persistence.TaskManagement.Repositories.Tasks;
using TE4IT.Persistence.TaskManagement.Repositories.TaskRelations;
using TE4IT.Persistence.TaskManagement.Repositories.UseCases;

namespace TE4IT.Persistence.TaskManagement.ServiceRegistrations;

public static class RelationalServiceRegistration
{
    public static IServiceCollection AddRelational(this IServiceCollection services, Action<DbContextOptionsBuilder> configureDb)
    {
        services.AddDbContext<AppDbContext>(configureDb);
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Register Read Repositories
        services.AddScoped<IModuleReadRepository, ModuleReadRepository>();
        services.AddScoped<IProjectReadRepository, ProjectReadRepository>();
        services.AddScoped<TE4IT.Abstractions.Persistence.Repositories.ProjectMembers.IProjectMemberReadRepository, TE4IT.Persistence.TaskManagement.Repositories.ProjectMembers.ProjectMemberReadRepository>();
        services.AddScoped<TE4IT.Abstractions.Persistence.Repositories.ProjectMembers.IProjectMemberWriteRepository, TE4IT.Persistence.TaskManagement.Repositories.ProjectMembers.ProjectMemberWriteRepository>();
        services.AddScoped<IProjectInvitationReadRepository, ProjectInvitationReadRepository>();
        services.AddScoped<ITaskReadRepository, TaskReadRepository>();
        services.AddScoped<IUseCaseReadRepository, UseCaseReadRepository>();

        // Register Write Repositories
        services.AddScoped<IModuleWriteRepository, ModuleWriteRepository>();
        services.AddScoped<IProjectWriteRepository, ProjectWriteRepository>();
        services.AddScoped<IProjectInvitationWriteRepository, ProjectInvitationWriteRepository>();
        services.AddScoped<ITaskWriteRepository, TaskWriteRepository>();
        services.AddScoped<IUseCaseWriteRepository, UseCaseWriteRepository>();
        services.AddScoped<ITaskRelationWriteRepository, TaskRelationWriteRepository>();

        return services;
    }
}