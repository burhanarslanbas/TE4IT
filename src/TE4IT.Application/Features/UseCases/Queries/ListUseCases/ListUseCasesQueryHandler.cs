using MediatR;
using TE4IT.Abstractions.Persistence.Repositories.Modules;
using TE4IT.Abstractions.Persistence.Repositories.Projects;
using TE4IT.Abstractions.Persistence.Repositories.Tasks;
using TE4IT.Abstractions.Persistence.Repositories.UseCases;
using TE4IT.Application.Abstractions.Auth;
using TE4IT.Application.Common.Pagination;
using TE4IT.Application.Features.UseCases.Responses;
using TE4IT.Domain.Exceptions.Common;
using TE4IT.Domain.Exceptions.Projects;
using TE4IT.Domain.Services;
using TE4IT.Domain.ValueObjects;

namespace TE4IT.Application.Features.UseCases.Queries.ListUseCases;

public sealed class ListUseCasesQueryHandler(
    IUseCaseReadRepository useCaseRepository,
    ITaskReadRepository taskRepository,
    IModuleReadRepository moduleReadRepository,
    IProjectReadRepository projectReadRepository,
    ICurrentUser currentUser,
    IUserPermissionService userPermissionService) : IRequestHandler<ListUseCasesQuery, PagedResult<UseCaseListItemResponse>>
{
    public async Task<PagedResult<UseCaseListItemResponse>> Handle(ListUseCasesQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = currentUser.Id ?? throw new UnauthorizedAccessException();
        
        // Modülü kontrol et
        var module = await moduleReadRepository.GetByIdAsync(request.ModuleId, cancellationToken);
        if (module is null)
            throw new ResourceNotFoundException("Module", request.ModuleId);

        // Projeyi kontrol et
        var project = await projectReadRepository.GetByIdAsync(module.ProjectId, cancellationToken);
        if (project is null)
            throw new ResourceNotFoundException("Project", module.ProjectId);

        // Erişim kontrolü: Kullanıcının projeye erişim yetkisi var mı?
        if (!userPermissionService.CanAccessProject(currentUserId, project))
            throw new ProjectAccessDeniedException(module.ProjectId, currentUserId.Value, "Projeye erişim yetkiniz bulunmamaktadır.");

        var page = await useCaseRepository.GetByModuleIdAsync(
            request.ModuleId,
            request.Page,
            request.PageSize,
            request.IsActive,
            request.Search,
            cancellationToken);

        var items = new List<UseCaseListItemResponse>();
        foreach (var useCase in page.Items)
        {
            var taskCount = await taskRepository.CountByUseCaseAsync(useCase.Id, cancellationToken);
            items.Add(new UseCaseListItemResponse
            {
                Id = useCase.Id,
                Title = useCase.Title,
                IsActive = useCase.IsActive,
                StartedDate = useCase.StartedDate,
                TaskCount = taskCount
            });
        }

        return new PagedResult<UseCaseListItemResponse>(items, page.TotalCount, page.Page, page.PageSize);
    }
}

