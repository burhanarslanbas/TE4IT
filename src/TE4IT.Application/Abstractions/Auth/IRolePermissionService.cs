namespace TE4IT.Application.Abstractions.Auth;

public interface IRolePermissionService
{
    IReadOnlyCollection<string> GetPermissionsForRoles(IEnumerable<string> roles);
}


