namespace TE4IT.Application.Features.Projects.Commands.InviteProjectMember;

public sealed record InviteProjectMemberCommandResponse
{
    public Guid InvitationId { get; init; }
    public string Email { get; init; } = string.Empty;
    public DateTime ExpiresAt { get; init; }
}

