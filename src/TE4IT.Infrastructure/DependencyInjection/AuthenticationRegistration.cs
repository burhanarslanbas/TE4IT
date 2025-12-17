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
                // Güçlü password policy
                opt.Password.RequireDigit = true;                    // ✅ Rakam zorunlu
                opt.Password.RequiredLength = 8;                     // ✅ Minimum 8 karakter
                opt.Password.RequireNonAlphanumeric = true;           // ✅ Özel karakter zorunlu
                opt.Password.RequireUppercase = true;                  // ✅ Büyük harf zorunlu
                opt.Password.RequireLowercase = true;                 // ✅ Küçük harf zorunlu
                opt.Password.RequiredUniqueChars = 3;                  // ✅ En az 3 farklı karakter türü
                
                // User ayarları
                opt.User.RequireUniqueEmail = true;
                
                // Güvenlik ayarları
                opt.Lockout.AllowedForNewUsers = true;                // ✅ Yeni kullanıcılar için lockout
                opt.Lockout.MaxFailedAccessAttempts = 5;              // ✅ 5 başarısız deneme
                opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15); // ✅ 15 dakika lockout
                
                // Güvenlik token ayarları
                opt.Tokens.PasswordResetTokenProvider = TokenOptions.DefaultProvider;
                opt.Tokens.EmailConfirmationTokenProvider = TokenOptions.DefaultProvider;
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

        var signingKey = config["Jwt:SigningKey"] ?? config["JWT_SIGNING_KEY"];
        var issuer = config["Jwt:Issuer"] ?? config["JWT_ISSUER"];
        var audience = config["Jwt:Audience"] ?? config["JWT_AUDIENCE"];
        
        // Test ortamında varsayılan değerler kullan
        if (string.IsNullOrWhiteSpace(signingKey))
        {
            signingKey = "ToAyJiE3OPBfbv7GN0elgLNxxEwmXtd+7EyC76yxiss="; // Test için varsayılan key
        }
        if (string.IsNullOrWhiteSpace(issuer))
        {
            issuer = "https://localhost:5001";
        }
        if (string.IsNullOrWhiteSpace(audience))
        {
            audience = "te4it-api";
        }
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
                ctx.User.IsInRole(TE4IT.Domain.Constants.RoleNames.Trial) ||
                ctx.User.HasClaim("permission", TE4IT.Domain.Constants.Permissions.Project.Create)
            ));
            o.AddPolicy("ProjectRead", policy => policy.RequireAssertion(ctx =>
                ctx.User.IsInRole(TE4IT.Domain.Constants.RoleNames.Administrator) ||
                ctx.User.IsInRole(TE4IT.Domain.Constants.RoleNames.OrganizationManager) ||
                ctx.User.IsInRole(TE4IT.Domain.Constants.RoleNames.TeamLead) ||
                ctx.User.IsInRole(TE4IT.Domain.Constants.RoleNames.Employee) ||
                ctx.User.IsInRole(TE4IT.Domain.Constants.RoleNames.Trial) ||
                ctx.User.HasClaim("permission", TE4IT.Domain.Constants.Permissions.Project.View)
            ));
            o.AddPolicy("ProjectUpdate", policy => policy.RequireAssertion(ctx =>
                ctx.User.IsInRole(TE4IT.Domain.Constants.RoleNames.Administrator) ||
                ctx.User.IsInRole(TE4IT.Domain.Constants.RoleNames.OrganizationManager) ||
                ctx.User.IsInRole(TE4IT.Domain.Constants.RoleNames.TeamLead) ||
                ctx.User.IsInRole(TE4IT.Domain.Constants.RoleNames.Trial) ||
                ctx.User.HasClaim("permission", TE4IT.Domain.Constants.Permissions.Project.Update)
            ));
            o.AddPolicy("ProjectDelete", policy => policy.RequireAssertion(ctx =>
                ctx.User.IsInRole(TE4IT.Domain.Constants.RoleNames.Administrator) ||
                ctx.User.IsInRole(TE4IT.Domain.Constants.RoleNames.OrganizationManager) ||
                ctx.User.IsInRole(TE4IT.Domain.Constants.RoleNames.TeamLead) ||
                ctx.User.IsInRole(TE4IT.Domain.Constants.RoleNames.Trial) ||
                ctx.User.HasClaim("permission", TE4IT.Domain.Constants.Permissions.Project.Delete)
            ));

            // Module policies
            o.AddPolicy("ModuleCreate", policy => policy.RequireAssertion(ctx =>
                ctx.User.IsInRole(TE4IT.Domain.Constants.RoleNames.Administrator) ||
                ctx.User.IsInRole(TE4IT.Domain.Constants.RoleNames.OrganizationManager) ||
                ctx.User.IsInRole(TE4IT.Domain.Constants.RoleNames.TeamLead) ||
                ctx.User.IsInRole(TE4IT.Domain.Constants.RoleNames.Trial) ||
                ctx.User.HasClaim("permission", TE4IT.Domain.Constants.Permissions.Module.Create)
            ));
            o.AddPolicy("ModuleRead", policy => policy.RequireAssertion(ctx =>
                ctx.User.IsInRole(TE4IT.Domain.Constants.RoleNames.Administrator) ||
                ctx.User.IsInRole(TE4IT.Domain.Constants.RoleNames.OrganizationManager) ||
                ctx.User.IsInRole(TE4IT.Domain.Constants.RoleNames.TeamLead) ||
                ctx.User.IsInRole(TE4IT.Domain.Constants.RoleNames.Employee) ||
                ctx.User.IsInRole(TE4IT.Domain.Constants.RoleNames.Trial) ||
                ctx.User.HasClaim("permission", TE4IT.Domain.Constants.Permissions.Module.View)
            ));
            o.AddPolicy("ModuleUpdate", policy => policy.RequireAssertion(ctx =>
                ctx.User.IsInRole(TE4IT.Domain.Constants.RoleNames.Administrator) ||
                ctx.User.IsInRole(TE4IT.Domain.Constants.RoleNames.OrganizationManager) ||
                ctx.User.IsInRole(TE4IT.Domain.Constants.RoleNames.TeamLead) ||
                ctx.User.IsInRole(TE4IT.Domain.Constants.RoleNames.Trial) ||
                ctx.User.HasClaim("permission", TE4IT.Domain.Constants.Permissions.Module.Update)
            ));
            o.AddPolicy("ModuleDelete", policy => policy.RequireAssertion(ctx =>
                ctx.User.IsInRole(TE4IT.Domain.Constants.RoleNames.Administrator) ||
                ctx.User.IsInRole(TE4IT.Domain.Constants.RoleNames.OrganizationManager) ||
                ctx.User.IsInRole(TE4IT.Domain.Constants.RoleNames.TeamLead) ||
                ctx.User.IsInRole(TE4IT.Domain.Constants.RoleNames.Trial) ||
                ctx.User.HasClaim("permission", TE4IT.Domain.Constants.Permissions.Module.Delete)
            ));

            // UseCase policies
            o.AddPolicy("UseCaseCreate", policy => policy.RequireAssertion(ctx =>
                ctx.User.IsInRole(TE4IT.Domain.Constants.RoleNames.Administrator) ||
                ctx.User.IsInRole(TE4IT.Domain.Constants.RoleNames.OrganizationManager) ||
                ctx.User.IsInRole(TE4IT.Domain.Constants.RoleNames.TeamLead) ||
                ctx.User.IsInRole(TE4IT.Domain.Constants.RoleNames.Trial) ||
                ctx.User.HasClaim("permission", TE4IT.Domain.Constants.Permissions.UseCase.Create)
            ));
            o.AddPolicy("UseCaseRead", policy => policy.RequireAssertion(ctx =>
                ctx.User.IsInRole(TE4IT.Domain.Constants.RoleNames.Administrator) ||
                ctx.User.IsInRole(TE4IT.Domain.Constants.RoleNames.OrganizationManager) ||
                ctx.User.IsInRole(TE4IT.Domain.Constants.RoleNames.TeamLead) ||
                ctx.User.IsInRole(TE4IT.Domain.Constants.RoleNames.Employee) ||
                ctx.User.IsInRole(TE4IT.Domain.Constants.RoleNames.Trial) ||
                ctx.User.HasClaim("permission", TE4IT.Domain.Constants.Permissions.UseCase.View)
            ));
            o.AddPolicy("UseCaseUpdate", policy => policy.RequireAssertion(ctx =>
                ctx.User.IsInRole(TE4IT.Domain.Constants.RoleNames.Administrator) ||
                ctx.User.IsInRole(TE4IT.Domain.Constants.RoleNames.OrganizationManager) ||
                ctx.User.IsInRole(TE4IT.Domain.Constants.RoleNames.TeamLead) ||
                ctx.User.IsInRole(TE4IT.Domain.Constants.RoleNames.Trial) ||
                ctx.User.HasClaim("permission", TE4IT.Domain.Constants.Permissions.UseCase.Update)
            ));
            o.AddPolicy("UseCaseDelete", policy => policy.RequireAssertion(ctx =>
                ctx.User.IsInRole(TE4IT.Domain.Constants.RoleNames.Administrator) ||
                ctx.User.IsInRole(TE4IT.Domain.Constants.RoleNames.OrganizationManager) ||
                ctx.User.IsInRole(TE4IT.Domain.Constants.RoleNames.TeamLead) ||
                ctx.User.IsInRole(TE4IT.Domain.Constants.RoleNames.Trial) ||
                ctx.User.HasClaim("permission", TE4IT.Domain.Constants.Permissions.UseCase.Delete)
            ));

            // Task policies
            o.AddPolicy("TaskCreate", policy => policy.RequireAssertion(ctx =>
                ctx.User.IsInRole(TE4IT.Domain.Constants.RoleNames.Administrator) ||
                ctx.User.IsInRole(TE4IT.Domain.Constants.RoleNames.OrganizationManager) ||
                ctx.User.IsInRole(TE4IT.Domain.Constants.RoleNames.TeamLead) ||
                ctx.User.IsInRole(TE4IT.Domain.Constants.RoleNames.Trial) ||
                ctx.User.HasClaim("permission", TE4IT.Domain.Constants.Permissions.Task.Create)
            ));
            o.AddPolicy("TaskRead", policy => policy.RequireAssertion(ctx =>
                ctx.User.IsInRole(TE4IT.Domain.Constants.RoleNames.Administrator) ||
                ctx.User.IsInRole(TE4IT.Domain.Constants.RoleNames.OrganizationManager) ||
                ctx.User.IsInRole(TE4IT.Domain.Constants.RoleNames.TeamLead) ||
                ctx.User.IsInRole(TE4IT.Domain.Constants.RoleNames.Employee) ||
                ctx.User.IsInRole(TE4IT.Domain.Constants.RoleNames.Trial) ||
                ctx.User.HasClaim("permission", TE4IT.Domain.Constants.Permissions.Task.View)
            ));
            o.AddPolicy("TaskUpdate", policy => policy.RequireAssertion(ctx =>
                ctx.User.IsInRole(TE4IT.Domain.Constants.RoleNames.Administrator) ||
                ctx.User.IsInRole(TE4IT.Domain.Constants.RoleNames.OrganizationManager) ||
                ctx.User.IsInRole(TE4IT.Domain.Constants.RoleNames.TeamLead) ||
                ctx.User.IsInRole(TE4IT.Domain.Constants.RoleNames.Trial) ||
                ctx.User.HasClaim("permission", TE4IT.Domain.Constants.Permissions.Task.Update)
            ));
            o.AddPolicy("TaskAssign", policy => policy.RequireAssertion(ctx =>
                ctx.User.IsInRole(TE4IT.Domain.Constants.RoleNames.Administrator) ||
                ctx.User.IsInRole(TE4IT.Domain.Constants.RoleNames.OrganizationManager) ||
                ctx.User.IsInRole(TE4IT.Domain.Constants.RoleNames.TeamLead) ||
                ctx.User.IsInRole(TE4IT.Domain.Constants.RoleNames.Trial) ||
                ctx.User.HasClaim("permission", TE4IT.Domain.Constants.Permissions.Task.Assign)
            ));
            o.AddPolicy("TaskStateChange", policy => policy.RequireAssertion(ctx =>
                ctx.User.IsInRole(TE4IT.Domain.Constants.RoleNames.Administrator) ||
                ctx.User.IsInRole(TE4IT.Domain.Constants.RoleNames.OrganizationManager) ||
                ctx.User.IsInRole(TE4IT.Domain.Constants.RoleNames.TeamLead) ||
                ctx.User.IsInRole(TE4IT.Domain.Constants.RoleNames.Employee) ||
                ctx.User.IsInRole(TE4IT.Domain.Constants.RoleNames.Trial) ||
                ctx.User.HasClaim("permission", TE4IT.Domain.Constants.Permissions.Task.StateChange)
            ));
            o.AddPolicy("TaskDelete", policy => policy.RequireAssertion(ctx =>
                ctx.User.IsInRole(TE4IT.Domain.Constants.RoleNames.Administrator) ||
                ctx.User.IsInRole(TE4IT.Domain.Constants.RoleNames.OrganizationManager) ||
                ctx.User.IsInRole(TE4IT.Domain.Constants.RoleNames.TeamLead) ||
                ctx.User.IsInRole(TE4IT.Domain.Constants.RoleNames.Trial) ||
                ctx.User.HasClaim("permission", TE4IT.Domain.Constants.Permissions.Task.Delete)
            ));

            // TaskRelation policies
            o.AddPolicy("TaskRelationCreate", policy => policy.RequireAssertion(ctx =>
                ctx.User.IsInRole(TE4IT.Domain.Constants.RoleNames.Administrator) ||
                ctx.User.IsInRole(TE4IT.Domain.Constants.RoleNames.OrganizationManager) ||
                ctx.User.IsInRole(TE4IT.Domain.Constants.RoleNames.TeamLead) ||
                ctx.User.IsInRole(TE4IT.Domain.Constants.RoleNames.Trial) ||
                ctx.User.HasClaim("permission", TE4IT.Domain.Constants.Permissions.TaskRelation.Create)
            ));
            o.AddPolicy("TaskRelationRead", policy => policy.RequireAssertion(ctx =>
                ctx.User.IsInRole(TE4IT.Domain.Constants.RoleNames.Administrator) ||
                ctx.User.IsInRole(TE4IT.Domain.Constants.RoleNames.OrganizationManager) ||
                ctx.User.IsInRole(TE4IT.Domain.Constants.RoleNames.TeamLead) ||
                ctx.User.IsInRole(TE4IT.Domain.Constants.RoleNames.Employee) ||
                ctx.User.IsInRole(TE4IT.Domain.Constants.RoleNames.Trial) ||
                ctx.User.HasClaim("permission", TE4IT.Domain.Constants.Permissions.TaskRelation.View)
            ));
            o.AddPolicy("TaskRelationDelete", policy => policy.RequireAssertion(ctx =>
                ctx.User.IsInRole(TE4IT.Domain.Constants.RoleNames.Administrator) ||
                ctx.User.IsInRole(TE4IT.Domain.Constants.RoleNames.OrganizationManager) ||
                ctx.User.IsInRole(TE4IT.Domain.Constants.RoleNames.TeamLead) ||
                ctx.User.IsInRole(TE4IT.Domain.Constants.RoleNames.Trial) ||
                ctx.User.HasClaim("permission", TE4IT.Domain.Constants.Permissions.TaskRelation.Delete)
            ));
        });
        return services;
    }
}


