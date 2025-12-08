using MediatR;

namespace TE4IT.Application.Features.Projects.Commands.AcceptProjectInvitation;

public sealed record AcceptProjectInvitationCommand(
    string Token) : IRequest<AcceptProjectInvitationCommandResponse>;

