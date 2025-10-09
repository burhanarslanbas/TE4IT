using TE4IT.Application.Abstractions.Auth;
using TE4IT.Domain.Constants;

namespace TE4IT.Infrastructure.Auth.Services.Authorization;

public sealed class RolePermissionService : IRolePermissionService
{
    public IReadOnlyCollection<string> GetPermissionsForRoles(IEnumerable<string> roles)
    {
        var set = new HashSet<string>();
        var rs = roles.ToHashSet(StringComparer.OrdinalIgnoreCase);

        if (rs.Contains(RoleNames.Administrator))
        {
            set.UnionWith(new[]
            {
                Permissions.Project.Create,
                Permissions.Project.View,
                Permissions.Project.Update,
                Permissions.Project.Delete
            });
        }

        if (rs.Contains(RoleNames.OrganizationManager) || rs.Contains(RoleNames.TeamLead))
        {
            set.UnionWith(new[]
            {
                Permissions.Project.Create,
                Permissions.Project.View,
                Permissions.Project.Update
            });
        }

        if (rs.Contains(RoleNames.Employee))
        {
            set.Add(Permissions.Project.View);
        }

        // Trainer, Customer örnekleri gerektikçe genişletilir
        return set;
    }
}

