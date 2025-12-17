using MediatR;
using TE4IT.Domain.Enums;

namespace TE4IT.Application.Features.Invitations.Commands.SendInvitation;

public sealed record SendInvitationCommand(
    Guid ProjectId,
    string Email,
    ProjectRole Role) : IRequest<SendInvitationCommandResponse>;
