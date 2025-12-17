using MediatR;

namespace TE4IT.Application.Features.Invitations.Commands.CancelInvitation;

public sealed record CancelInvitationCommand(
    Guid ProjectId,
    Guid InvitationId) : IRequest<bool>;
