using MediatR;

namespace TE4IT.Application.Features.Invitations.Commands.AcceptInvitation;

public sealed record AcceptInvitationCommand(
    string Token) : IRequest<AcceptInvitationCommandResponse>;
