namespace TE4IT.Application.Features.Invitations.Commands.SendInvitation;

public sealed record SendInvitationCommandResponse
{
    public Guid InvitationId { get; init; }
    public string Email { get; init; } = string.Empty;
    public DateTime ExpiresAt { get; init; }
}
