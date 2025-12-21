using TE4IT.Domain.Enums;

namespace TE4IT.Application.Features.Invitations.Commands.SendInvitation;

public sealed record SendInvitationRequest(
    Guid ProjectId,
    string Email,
    ProjectRole Role);
