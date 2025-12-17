using MediatR;

namespace TE4IT.Application.Features.Invitations.Queries.GetInvitationByToken;

public sealed record GetInvitationByTokenQuery(
    string Token) : IRequest<InvitationResponse?>;
