namespace TE4IT.Application.Features.Projects.Commands.AddProjectMember;

public sealed record AddProjectMemberCommandResponse
{
    public Guid ProjectMemberId { get; init; }
}

