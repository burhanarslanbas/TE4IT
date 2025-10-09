using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using TE4IT.Domain.Constants;

namespace TE4IT.Infrastructure.Auth.Services;

/// <summary>
/// Rol seed işlemlerini yöneten servis
/// </summary>
public sealed class RoleSeeder(RoleManager<IdentityRole<Guid>> roleManager, ILogger<RoleSeeder> logger)
{
    /// <summary>
    /// Varsayılan rolleri oluşturur (idempotent)
    /// </summary>
    public async Task SeedDefaultRolesAsync(CancellationToken cancellationToken = default)
    {
        var defaultRoles = new[]
        {
            RoleNames.Administrator,
            RoleNames.OrganizationManager,
            RoleNames.TeamLead,
            RoleNames.Employee,
            RoleNames.Trainer,
            RoleNames.Customer
        };

        foreach (var roleName in defaultRoles)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                var role = new IdentityRole<Guid>
                {
                    Id = Guid.NewGuid(),
                    Name = roleName,
                    NormalizedName = roleName.ToUpperInvariant()
                };

                var result = await roleManager.CreateAsync(role);
                if (result.Succeeded)
                {
                    logger.LogInformation("Role '{RoleName}' created successfully", roleName);
                }
                else
                {
                    logger.LogError("Failed to create role '{RoleName}': {Errors}", 
                        roleName, string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }
            else
            {
                logger.LogDebug("Role '{RoleName}' already exists, skipping", roleName);
            }
        }
    }
}
