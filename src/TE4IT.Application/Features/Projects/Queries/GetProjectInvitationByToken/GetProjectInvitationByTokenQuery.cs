using MediatR;

namespace TE4IT.Application.Features.Projects.Queries.GetProjectInvitationByToken;

public sealed record GetProjectInvitationByTokenQuery(
    string Token) : IRequest<ProjectInvitationResponse?>;

