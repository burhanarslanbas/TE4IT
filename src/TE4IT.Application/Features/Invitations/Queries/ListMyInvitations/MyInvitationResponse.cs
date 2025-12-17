using TE4IT.Domain.Enums;

namespace TE4IT.Application.Features.Invitations.Queries.ListMyInvitations;

public sealed record MyInvitationResponse(
    Guid InvitationId,
    Guid ProjectId,
    string ProjectTitle,
    ProjectRole Role,
    DateTime ExpiresAt,
    string? InvitedByUserName,
    DateTime CreatedDate,
    string Status,
    DateTime? AcceptedAt,
    DateTime? CancelledAt);
