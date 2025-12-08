using TE4IT.Domain.Enums;

namespace TE4IT.Application.Features.Projects.Queries.ListProjectInvitations;

public sealed record ProjectInvitationListItemResponse
{
    public Guid InvitationId { get; init; }
    public string Email { get; init; } = string.Empty;
    public ProjectRole Role { get; init; }
    public string Status { get; init; } = string.Empty;
    public DateTime CreatedDate { get; init; }
    public DateTime ExpiresAt { get; init; }
    public DateTime? AcceptedAt { get; init; }
    public string InvitedByUserName { get; init; } = string.Empty;
}

