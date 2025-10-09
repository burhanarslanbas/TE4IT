using MediatR;

namespace TE4IT.Application.Features.Auth.Commands.Roles.UpdateRole;

/// <summary>
/// Rol güncelleme komutu
/// </summary>
public sealed record UpdateRoleCommand(Guid RoleId, string RoleName) : IRequest<UpdateRoleCommandResponse>;

