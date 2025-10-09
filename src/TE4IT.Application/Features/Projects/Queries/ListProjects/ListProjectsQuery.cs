using MediatR;
using TE4IT.Application.Common.Pagination;
using TE4IT.Application.Features.Projects.Responses;

namespace TE4IT.Application.Features.Projects.Queries.ListProjects;

public sealed record ListProjectsQuery(int Page = 1, int PageSize = 20) : IRequest<PagedResult<ProjectListItemResponse>>;
