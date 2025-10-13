using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using TE4IT.Application.Abstractions.Auth;
using TE4IT.Infrastructure.Auth.Services;
using TE4IT.Infrastructure.Auth.Services.Authorization;

namespace TE4IT.Infrastructure;

public static class ServiceRegistration
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        // Infrastructure servisleri (ör. CurrentUser, TokenService, Email/SMS vb.)
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUser, CurrentUserService>();
        services.AddTransient<ITokenService, JwtTokenService>();
        services.AddScoped<IRefreshTokenService, RefreshTokenService>();
        services.AddScoped<IUserInfoService, UserInfoService>();
        services.AddScoped<IUserAccountService, UserAccountService>();
        services.AddScoped<IRolePermissionService, RolePermissionService>();
        services.AddScoped<IPolicyAuthorizer, PolicyAuthorizer>();
        services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();

        // MediatR Handlers from Infrastructure - Infrastructure katmanındaki handler'ları register et
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ServiceRegistration).Assembly));

        // Add Role Seeder
        services.AddScoped<TE4IT.Infrastructure.Auth.Services.RoleSeeder>();

        return services;
    }
}


