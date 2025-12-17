using MediatR;
using TE4IT.Abstractions.Persistence.Repositories.Projects;
using TE4IT.Application.Abstractions.Auth;
using TE4IT.Application.Common.Pagination;
using TE4IT.Application.Features.Projects.Responses;
using TE4IT.Domain.Constants;
using TE4IT.Domain.Services;

namespace TE4IT.Application.Features.Projects.Queries.ListProjects;

public sealed class ListProjectsQueryHandler(
    IProjectReadRepository projectRepository,
    ICurrentUser currentUser,
    IUserPermissionService userPermissionService) : IRequestHandler<ListProjectsQuery, PagedResult<ProjectListItemResponse>>
{
    public async Task<PagedResult<ProjectListItemResponse>> Handle(ListProjectsQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = currentUser.Id ?? throw new UnauthorizedAccessException();

        // Admin kontrolü
        var isAdmin = userPermissionService.IsSystemAdministrator(currentUserId);

        // Kullanıcının erişebildiği projeleri getir
        var page = await projectRepository.GetByUserAccessAsync(
            currentUserId.Value,
            isAdmin,
            request.Page,
            request.PageSize,
            request.IsActive,
            request.Search,
            cancellationToken);

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
