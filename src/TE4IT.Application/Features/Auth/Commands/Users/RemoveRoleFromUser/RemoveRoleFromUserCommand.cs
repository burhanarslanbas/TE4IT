using MediatR;

namespace TE4IT.Application.Features.Auth.Commands.Users.RemoveRoleFromUser;

/// <summary>
/// Kullanıcıdan rol kaldırma komutu
/// </summary>
public sealed record RemoveRoleFromUserCommand(Guid UserId, string RoleName)
    : IRequest;

