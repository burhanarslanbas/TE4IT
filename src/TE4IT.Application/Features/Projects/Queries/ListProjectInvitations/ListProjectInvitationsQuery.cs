using MediatR;

namespace TE4IT.Application.Features.Projects.Queries.ListProjectInvitations;

public sealed record ListProjectInvitationsQuery(
    Guid ProjectId,
    string? Status = null) : IRequest<List<ProjectInvitationListItemResponse>>;

