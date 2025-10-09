using MediatR;
using TE4IT.Abstractions.Persistence.Repositories.Projects;
using TE4IT.Application.Common.Pagination;
using TE4IT.Application.Features.Projects.Responses;

namespace TE4IT.Application.Features.Projects.Queries.ListProjects;

public sealed class ListProjectsQueryHandler(IProjectReadRepository projectRepository) : IRequestHandler<ListProjectsQuery, PagedResult<ProjectListItemResponse>>
{
    public async Task<PagedResult<ProjectListItemResponse>> Handle(ListProjectsQuery request, CancellationToken cancellationToken)
    {
        var page = await projectRepository.ListAsync(request.Page, request.PageSize, cancellationToken);
        var items = page.Items.Select(p => new ProjectListItemResponse
        {
            Id = p.Id,
            Title = p.Title,
            IsActive = p.IsActive,
            StartedDate = p.StartedDate
        }).ToList();
        return new PagedResult<ProjectListItemResponse>(items, page.TotalCount, page.Page, page.PageSize);
    }
}
