using MediatR;

namespace TE4IT.Application.Features.Auth.Commands.Roles.CreateRole;

/// <summary>
/// Yeni rol oluşturma komutu
/// </summary>
public sealed record CreateRoleCommand(string RoleName) : IRequest<CreateRoleCommandResponse>;

