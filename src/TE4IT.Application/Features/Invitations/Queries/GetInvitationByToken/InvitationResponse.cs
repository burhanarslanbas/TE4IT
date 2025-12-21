using TE4IT.Domain.Enums;

namespace TE4IT.Application.Features.Invitations.Queries.GetInvitationByToken;

public sealed record InvitationResponse
{
    public Guid InvitationId { get; init; }
    public Guid ProjectId { get; init; }
    public string ProjectTitle { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public ProjectRole Role { get; init; }
    public DateTime ExpiresAt { get; init; }
    public string InvitedByUserName { get; init; } = string.Empty;
}
