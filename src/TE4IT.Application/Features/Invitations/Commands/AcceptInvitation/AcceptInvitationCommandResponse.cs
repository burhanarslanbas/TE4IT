namespace TE4IT.Application.Features.Invitations.Commands.AcceptInvitation;

public sealed record AcceptInvitationCommandResponse
{
    public Guid ProjectMemberId { get; init; }
    public Guid ProjectId { get; init; }
}
