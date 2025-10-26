using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using TE4IT.Infrastructure.Auth.Services;
using TE4IT.Persistence.Common.Contexts;
using TE4IT.Persistence.Common.Identity;

namespace TE4IT.Infrastructure;

public static class AuthenticationRegistration
{
    public static IServiceCollection AddIdentityAndJwtAuthentication(this IServiceCollection services, IConfiguration config)
    {
        services
            .AddIdentity<AppUser, IdentityRole<Guid>>(opt =>
            {
                opt.Password.RequireDigit = false;
                opt.Password.RequiredLength = 6;
                opt.Password.RequireNonAlphanumeric = false;
                opt.Password.RequireUppercase = false;
                opt.Password.RequireLowercase = false;
                opt.User.RequireUniqueEmail = true;
                opt.Lockout.AllowedForNewUsers = false;
                opt.Lockout.MaxFailedAccessAttempts = 10;
                opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

        // Ensure API returns 401/403 instead of redirecting to login pages
        services.ConfigureApplicationCookie(opt =>
        {
            opt.Events.OnRedirectToLogin = ctx =>
            {
                ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Task.CompletedTask;
            };
            opt.Events.OnRedirectToAccessDenied = ctx =>
            {
                ctx.Response.StatusCode = StatusCodes.Status403Forbidden;
                return Task.CompletedTask;
            };
        });

        var signingKey = config["Jwt:SigningKey"] ?? config["JWT_SIGNING_KEY"]!;
        var issuer = config["Jwt:Issuer"] ?? config["JWT_ISSUER"]!;
        var audience = config["Jwt:Audience"] ?? config["JWT_AUDIENCE"]!;
        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(5), // 5 dakika tolerans
                    NameClaimType = ClaimTypes.NameIdentifier,
                    RoleClaimType = ClaimTypes.Role,
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    IssuerSigningKey = signingKey.CreateSymmetricKey(),
                    RequireExpirationTime = true,
                    RequireSignedTokens = true
                };

                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = async context =>
                    {
                        var userIdClaim = context.Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                        if (!Guid.TryParse(userIdClaim, out var userId))
                        {
                            context.Fail("Invalid token subject");
                            return;
                        }

                        var userManager = context.HttpContext.RequestServices.GetRequiredService<UserManager<AppUser>>();
                        var user = await userManager.FindByIdAsync(userId.ToString());
                        if (user is null)
                        {
                            context.Fail("User not found");
                            return;
                        }

                        if (await userManager.IsLockedOutAsync(user))
                        {
                            context.Fail("User is locked out");
                            return;
                        }

                        // permissions_version denetimi: SecurityStamp değişmişse token reddedilir
                        var tokenVersion = context.Principal?.FindFirst("permissions_version")?.Value;
                        var currentVersion = await userManager.GetSecurityStampAsync(user);

                        // Sadece her iki değer de varsa ve farklıysa fail et
                        if (!string.IsNullOrEmpty(currentVersion) &&
                            !string.IsNullOrEmpty(tokenVersion) &&
                            tokenVersion != currentVersion)
                        {
                            context.Fail("Permissions version mismatch");
                            return;
                        }
                    }
                };
            });

        services.AddAuthorization(o =>
        {
            o.AddPolicy("ProjectCreate", policy => policy.RequireAssertion(ctx =>
                ctx.User.IsInRole(TE4IT.Domain.Constants.RoleNames.Administrator) ||
                ctx.User.IsInRole(TE4IT.Domain.Constants.RoleNames.OrganizationManager) ||
                ctx.User.IsInRole(TE4IT.Domain.Constants.RoleNames.TeamLead) ||
                ctx.User.HasClaim("permission", TE4IT.Domain.Constants.Permissions.Project.Create)
            ));
        });
        return services;
    }
}


