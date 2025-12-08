using MediatR;
using TE4IT.Application.Features.Projects.Responses;

namespace TE4IT.Application.Features.Projects.Queries.ListProjectMembers;

public sealed record ListProjectMembersQuery(Guid ProjectId) : IRequest<List<ProjectMemberResponse>>;

