using MediatR;

namespace TE4IT.Application.Features.Auth.Commands.Roles.DeleteRole;

/// <summary>
/// Rol silme komutu
/// </summary>
public sealed record DeleteRoleCommand(Guid RoleId) : IRequest;

