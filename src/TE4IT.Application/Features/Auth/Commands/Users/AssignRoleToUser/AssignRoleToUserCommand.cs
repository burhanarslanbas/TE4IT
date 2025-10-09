using MediatR;

namespace TE4IT.Application.Features.Auth.Commands.Users.AssignRoleToUser;

/// <summary>
/// Kullanıcıya rol atama komutu
/// </summary>
public sealed record AssignRoleToUserCommand(Guid UserId, string RoleName)
    : IRequest<AssignRoleToUserCommandResponse>;

