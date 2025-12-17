using MediatR;

namespace TE4IT.Application.Features.Invitations.Queries.ListInvitations;

public sealed record ListInvitationsQuery(
    Guid ProjectId,
    string? Status = null) : IRequest<List<InvitationListItemResponse>>;
