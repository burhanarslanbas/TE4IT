using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
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
        try
        {
            logger.LogInformation("Starting role seeding process...");

            var defaultRoles = new[]
            {
                RoleNames.Administrator,
                RoleNames.OrganizationManager,
                RoleNames.TeamLead,
                RoleNames.Employee,
                RoleNames.Trainer,
                RoleNames.Customer
            };

            // Role -> permission claims mapping
            var rolePermissions = new Dictionary<string, string[]>
            {
                [RoleNames.Administrator] = new[]
                {
                    Permissions.Project.Create,
                    Permissions.Project.View,
                    Permissions.Project.Update,
                    Permissions.Project.Delete
                },
                [RoleNames.OrganizationManager] = new[]
                {
                    Permissions.Project.Create,
                    Permissions.Project.View,
                    Permissions.Project.Update
                },
                [RoleNames.TeamLead] = new[]
                {
                    Permissions.Project.View,
                    Permissions.Project.Update
                },
                [RoleNames.Employee] = new[]
                {
                    Permissions.Project.View
                },
                [RoleNames.Trainer] = new[]
                {
                    Permissions.Project.View
                },
                [RoleNames.Customer] = new[]
                {
                    Permissions.Project.View
                },
            };

            foreach (var roleName in defaultRoles)
            {
                try
                {
                    logger.LogDebug("Checking if role '{RoleName}' exists...", roleName);
                    
                    var exists = await roleManager.RoleExistsAsync(roleName);
                    if (!exists)
                    {
                        logger.LogInformation("Creating role '{RoleName}'...", roleName);
                        
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

                    // Ensure permission claims exist for the role
                    var roleEntity = await roleManager.FindByNameAsync(roleName);
                    if (roleEntity is not null && rolePermissions.TryGetValue(roleName, out var permissions))
                    {
                        var existingClaims = await roleManager.GetClaimsAsync(roleEntity);
                        var existingPermissionValues = new HashSet<string>(
                            existingClaims.Where(c => c.Type == "permission").Select(c => c.Value)
                        );

                        foreach (var permission in permissions)
                        {
                            if (!existingPermissionValues.Contains(permission))
                            {
                                var addClaimResult = await roleManager.AddClaimAsync(roleEntity, new Claim("permission", permission));
                                if (addClaimResult.Succeeded)
                                {
                                    logger.LogInformation("Added claim '{Permission}' to role '{RoleName}'", permission, roleName);
                                }
                                else
                                {
                                    logger.LogError("Failed to add claim '{Permission}' to role '{RoleName}': {Errors}",
                                        permission,
                                        roleName,
                                        string.Join(", ", addClaimResult.Errors.Select(e => e.Description)));
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error while processing role '{RoleName}'", roleName);
                    // Continue with next role instead of failing completely
                }
            }

            logger.LogInformation("Role seeding process completed successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Critical error during role seeding process");
            throw; // Re-throw to let the application know seeding failed
        }
    }
}
