namespace TE4IT.Application.Features.Auth.Queries.Roles.GetAllRoles;

/// <summary>
/// Rol bilgileri DTO
/// </summary>
public sealed class RoleResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
}

