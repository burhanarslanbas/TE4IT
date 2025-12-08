using MediatR;
using TE4IT.Abstractions.Persistence.Repositories.Modules;
using TE4IT.Abstractions.Persistence.Repositories.Projects;
using TE4IT.Abstractions.Persistence.Repositories.UseCases;
using TE4IT.Application.Abstractions.Auth;
using TE4IT.Application.Common.Pagination;
using TE4IT.Application.Features.Modules.Responses;
using TE4IT.Domain.Exceptions.Common;
using TE4IT.Domain.Exceptions.Projects;
using TE4IT.Domain.Services;
using TE4IT.Domain.ValueObjects;

namespace TE4IT.Application.Features.Modules.Queries.ListModules;

public sealed class ListModulesQueryHandler(
    IModuleReadRepository moduleRepository,
    IUseCaseReadRepository useCaseRepository,
    IProjectReadRepository projectReadRepository,
    ICurrentUser currentUser,
    IUserPermissionService userPermissionService) : IRequestHandler<ListModulesQuery, PagedResult<ModuleListItemResponse>>
{
    public async Task<PagedResult<ModuleListItemResponse>> Handle(ListModulesQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = currentUser.Id ?? throw new UnauthorizedAccessException();
        
        // Projeyi kontrol et
        var project = await projectReadRepository.GetByIdAsync(request.ProjectId, cancellationToken);
        if (project is null)
            throw new ResourceNotFoundException("Project", request.ProjectId);

        // Erişim kontrolü: Kullanıcının projeye erişim yetkisi var mı?
        if (!userPermissionService.CanAccessProject(currentUserId, project))
            throw new ProjectAccessDeniedException(request.ProjectId, currentUserId.Value, "Projeye erişim yetkiniz bulunmamaktadır.");

        var page = await moduleRepository.GetByProjectIdAsync(
            request.ProjectId,
            request.Page,
            request.PageSize,
            request.IsActive,
            request.Search,
            cancellationToken);

        var items = new List<ModuleListItemResponse>();
        foreach (var module in page.Items)
        {
            var useCaseCount = await useCaseRepository.CountByModuleAsync(module.Id, cancellationToken);
            items.Add(new ModuleListItemResponse
            {
                Id = module.Id,
                Title = module.Title,
                IsActive = module.IsActive,
                StartedDate = module.StartedDate,
                UseCaseCount = useCaseCount
            });
        }

        return new PagedResult<ModuleListItemResponse>(items, page.TotalCount, page.Page, page.PageSize);
    }
}

