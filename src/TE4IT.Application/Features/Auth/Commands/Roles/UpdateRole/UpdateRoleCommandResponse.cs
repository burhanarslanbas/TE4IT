using MediatR;

namespace TE4IT.Application.Features.Auth.Commands.Roles.UpdateRole;

/// <summary>
/// Rol güncelleme sonucu
/// </summary>
public sealed class UpdateRoleCommandResponse
{
    public Guid RoleId {get;init;}
    public string RoleName {get;init;} = string.Empty;
    public bool Success {get;init;}
}

