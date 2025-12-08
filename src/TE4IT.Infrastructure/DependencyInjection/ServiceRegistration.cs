using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TE4IT.Application.Abstractions.Auth;
using TE4IT.Application.Abstractions.Email;
using TE4IT.Application.Abstractions.Common;
using TE4IT.Infrastructure.Auth.Services;
using TE4IT.Infrastructure.Auth.Services.Authorization;
using TE4IT.Infrastructure.Email.Services;
using TE4IT.Infrastructure.Common;
using TE4IT.Infrastructure.Options;
using Microsoft.Extensions.Options;

namespace TE4IT.Infrastructure;

public static class ServiceRegistration
{
	public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
	{
		// Infrastructure servisleri (ör. CurrentUser, TokenService, Email/SMS vb.)
		services.AddHttpContextAccessor();
		services.AddScoped<ICurrentUser, CurrentUserService>();
		services.AddTransient<ITokenService, JwtTokenService>();
		services.AddScoped<IRefreshTokenService, RefreshTokenService>();
		services.AddScoped<IUserInfoService, UserInfoService>();
		services.AddScoped<IUserAccountService, UserAccountService>();
		services.AddScoped<IRolePermissionService, RolePermissionService>();
		services.AddScoped<Domain.Services.IUserPermissionService, UserPermissionService>();
		services.AddScoped<IPolicyAuthorizer, PolicyAuthorizer>();
		services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();

		// MediatR Handlers from Infrastructure - Infrastructure katmanındaki handler'ları register et
		services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ServiceRegistration).Assembly));

		// Add Role Seeder
		services.AddScoped<RoleSeeder>();

		// Add Email Options (minimal config: Username + Password)
		services.Configure<EmailOptions>(configuration.GetSection("Email"));
		
		// Add Project Invitation Options
		services.Configure<ProjectInvitationOptions>(configuration.GetSection("ProjectInvitation"));
		
		// Azure App Service için environment variables'dan email settings oku
		services.PostConfigure<EmailOptions>(opt =>
		{
			if (string.IsNullOrWhiteSpace(opt.Username))
				opt.Username = configuration["EMAIL_USERNAME"] ?? "";
			if (string.IsNullOrWhiteSpace(opt.Password))
				opt.Password = configuration["EMAIL_PASSWORD"] ?? "";
		});
		services.PostConfigure<EmailOptions>(opt =>
		{
			// Sensible defaults for Gmail
			if (string.IsNullOrWhiteSpace(opt.Host)) opt.Host = "smtp.gmail.com";
			if (opt.Port == 0) opt.Port = 587;
			opt.EnableSsl = true;
			// If From not provided, use Username
			if (string.IsNullOrWhiteSpace(opt.From) && !string.IsNullOrWhiteSpace(opt.Username))
			{
				opt.From = opt.Username;
			}
		});
		services.AddScoped<IEmailSender, EmailSenderService>();
		services.AddScoped<IEmailTemplateService, EmailTemplateService>();
		
		// URL Service
		services.AddScoped<IUrlService, UrlService>();

		// Invitation Token Service
		services.AddScoped<IInvitationTokenService, InvitationTokenService>();

		return services;
	}
}


