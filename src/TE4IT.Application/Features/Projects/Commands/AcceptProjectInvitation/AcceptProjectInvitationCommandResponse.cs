namespace TE4IT.Application.Features.Projects.Commands.AcceptProjectInvitation;

public sealed record AcceptProjectInvitationCommandResponse
{
    public Guid ProjectMemberId { get; init; }
    public Guid ProjectId { get; init; }
}

