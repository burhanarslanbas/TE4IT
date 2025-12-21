using MediatR;

namespace TE4IT.Application.Features.Invitations.Queries.ListMyPendingInvitations;

public sealed record ListMyPendingInvitationsQuery() : IRequest<List<MyPendingInvitationResponse>>;
