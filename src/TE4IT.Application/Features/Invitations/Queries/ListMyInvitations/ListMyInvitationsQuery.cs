using MediatR;

namespace TE4IT.Application.Features.Invitations.Queries.ListMyInvitations;

public sealed record ListMyInvitationsQuery(string? Status = null) : IRequest<List<MyInvitationResponse>>;
