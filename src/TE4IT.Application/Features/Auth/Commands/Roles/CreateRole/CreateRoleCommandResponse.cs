namespace TE4IT.Application.Features.Auth.Commands.Roles.CreateRole;

/// <summary>
/// Rol oluşturma sonucu
/// </summary>
public sealed class CreateRoleCommandResponse
{
    public Guid RoleId { get; init; }
    public string RoleName { get; init; } = string.Empty;
    public bool Success { get; init; }
}

