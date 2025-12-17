using TE4IT.Domain.Enums;

namespace TE4IT.Application.Features.Invitations.Queries.ListMyPendingInvitations;

public sealed record MyPendingInvitationResponse(
    Guid InvitationId,
    Guid ProjectId,
    string ProjectTitle,
    ProjectRole Role,
    DateTime ExpiresAt,
    string InvitedByUserName,
    DateTime CreatedDate);
