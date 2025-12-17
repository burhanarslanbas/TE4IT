using MediatR;

namespace TE4IT.Application.Features.Projects.Commands.CancelProjectInvitation;

public sealed record CancelProjectInvitationCommand(
    Guid ProjectId,
    Guid InvitationId) : IRequest<bool>;

