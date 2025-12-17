using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TE4IT.Domain.Constants;
using TE4IT.Persistence.Common.Contexts;
using TE4IT.Persistence.Common.Identity;
using Xunit;

namespace TE4IT.Tests.Integration.Common;

/// <summary>
/// Integration testler için base class
/// Her test için izole bir In-Memory DbContext sağlar
/// </summary>
public abstract class IntegrationTestBase : IDisposable
{
    protected AppDbContext DbContext { get; }
    protected UserManager<AppUser> UserManager { get; }
    protected RoleManager<IdentityRole<Guid>> RoleManager { get; }

    protected IntegrationTestBase()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        DbContext = new AppDbContext(options);

        // Setup UserManager for Identity operations
        var userStore = new UserStore<AppUser, IdentityRole<Guid>, AppDbContext, Guid>(DbContext);
        var roleStore = new RoleStore<IdentityRole<Guid>, AppDbContext, Guid>(DbContext);
        var passwordHasher = new PasswordHasher<AppUser>();
        var userValidators = new List<IUserValidator<AppUser>>();
        // Add default password validators to enforce password policy
        var passwordValidators = new List<IPasswordValidator<AppUser>>
        {
            new PasswordValidator<AppUser>()
        };
        var roleValidators = new List<IRoleValidator<IdentityRole<Guid>>>();
        var lookupNormalizer = new UpperInvariantLookupNormalizer();
        var errors = new IdentityErrorDescriber();
        var services = new ServiceCollection();
        services.AddLogging();
        var serviceProvider = services.BuildServiceProvider();
        var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
        var userLogger = loggerFactory.CreateLogger<UserManager<AppUser>>();
        var roleLogger = loggerFactory.CreateLogger<RoleManager<IdentityRole<Guid>>>();

        var identityOptions = Options.Create(new IdentityOptions
        {
            Password = new PasswordOptions
            {
                RequireDigit = true,
                RequiredLength = 8,
                RequireNonAlphanumeric = true,
                RequireUppercase = true,
                RequireLowercase = true,
                RequiredUniqueChars = 3
            },
            Lockout = new LockoutOptions
            {
                AllowedForNewUsers = true,
                MaxFailedAccessAttempts = 5,
                DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15)
            }
        });

        UserManager = new UserManager<AppUser>(
            userStore,
            identityOptions,
            passwordHasher,
            userValidators,
            passwordValidators,
            lookupNormalizer,
            errors,
            serviceProvider,
            userLogger);

        RoleManager = new RoleManager<IdentityRole<Guid>>(
            roleStore,
            roleValidators,
            lookupNormalizer,
            errors,
            roleLogger);

        // Seed default roles
        SeedDefaultRolesAsync().GetAwaiter().GetResult();
    }

    private async Task SeedDefaultRolesAsync()
    {
        var defaultRoles = new[]
        {
            RoleNames.Administrator,
            RoleNames.OrganizationManager,
            RoleNames.TeamLead,
            RoleNames.Employee,
            RoleNames.Trainer,
            RoleNames.Customer,
            RoleNames.Trial
        };

        foreach (var roleName in defaultRoles)
        {
            var exists = await RoleManager.RoleExistsAsync(roleName);
            if (!exists)
            {
                var role = new IdentityRole<Guid>
                {
                    Id = Guid.NewGuid(),
                    Name = roleName,
                    NormalizedName = roleName.ToUpperInvariant()
                };
                await RoleManager.CreateAsync(role);
            }
        }
    }

    protected async Task SeedDataAsync(params object[] entities)
    {
        foreach (var entity in entities)
        {
            await DbContext.AddAsync(entity);
        }
        await DbContext.SaveChangesAsync();
    }

    protected async Task<AppUser> SeedUserAsync(string userName, string email, string password, string? role = null)
    {
        var user = new AppUser
        {
            Id = Guid.NewGuid(),
            UserName = userName,
            Email = email,
            NormalizedUserName = userName.ToUpperInvariant(),
            NormalizedEmail = email.ToUpperInvariant(),
            EmailConfirmed = true
        };

        var result = await UserManager.CreateAsync(user, password);
        if (!result.Succeeded)
        {
            throw new InvalidOperationException($"Failed to create user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }

        if (!string.IsNullOrEmpty(role))
        {
            await UserManager.AddToRoleAsync(user, role);
        }
        else
        {
            // Default to Trial role
            await UserManager.AddToRoleAsync(user, RoleNames.Trial);
        }

        await DbContext.SaveChangesAsync();
        return user;
    }

    protected async Task SeedRefreshTokenAsync(Guid userId, string token, DateTime expiresAt, string? ipAddress = null)
    {
        var refreshToken = new TE4IT.Persistence.Common.Identity.RefreshToken
        {
            UserId = userId,
            Token = token,
            TokenHash = Convert.ToBase64String(System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(token))),
            ExpiresAt = expiresAt,
            CreatedAt = DateTime.UtcNow,
            CreatedByIp = ipAddress ?? "127.0.0.1"
        };

        await DbContext.Set<TE4IT.Persistence.Common.Identity.RefreshToken>().AddAsync(refreshToken);
        await DbContext.SaveChangesAsync();
    }

    protected async Task ClearDatabaseAsync()
    {
        DbContext.Projects.RemoveRange(DbContext.Projects);
        DbContext.Modules.RemoveRange(DbContext.Modules);
        DbContext.UseCases.RemoveRange(DbContext.UseCases);
        DbContext.Tasks.RemoveRange(DbContext.Tasks);
        DbContext.TaskRelations.RemoveRange(DbContext.TaskRelations);
        DbContext.Set<TE4IT.Persistence.Common.Identity.RefreshToken>().RemoveRange(DbContext.Set<TE4IT.Persistence.Common.Identity.RefreshToken>());
        
        // Clear Identity tables
        var users = await UserManager.Users.ToListAsync();
        foreach (var user in users)
        {
            await UserManager.DeleteAsync(user);
        }

        await DbContext.SaveChangesAsync();
    }

    public void Dispose()
    {
        DbContext.Dispose();
    }
}

