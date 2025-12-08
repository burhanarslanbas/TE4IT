using MediatR;
using TE4IT.Abstractions.Persistence.Repositories.Modules;
using TE4IT.Abstractions.Persistence.Repositories.Projects;
using TE4IT.Abstractions.Persistence.Repositories.Tasks;
using TE4IT.Abstractions.Persistence.Repositories.UseCases;
using TE4IT.Application.Abstractions.Auth;
using TE4IT.Application.Common.Pagination;
using TE4IT.Application.Features.Tasks.Responses;
using TE4IT.Domain.Exceptions.Common;
using TE4IT.Domain.Exceptions.Projects;
using TE4IT.Domain.Services;
using TE4IT.Domain.ValueObjects;

namespace TE4IT.Application.Features.Tasks.Queries.ListTasks;

public sealed class ListTasksQueryHandler(
    ITaskReadRepository taskRepository,
    IUserInfoService userInfoService,
    IUseCaseReadRepository useCaseReadRepository,
    IModuleReadRepository moduleReadRepository,
    IProjectReadRepository projectReadRepository,
    ICurrentUser currentUser,
    IUserPermissionService userPermissionService) : IRequestHandler<ListTasksQuery, PagedResult<TaskListItemResponse>>
{
    public async Task<PagedResult<TaskListItemResponse>> Handle(ListTasksQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = currentUser.Id ?? throw new UnauthorizedAccessException();
        
        // UseCase'i kontrol et
        var useCase = await useCaseReadRepository.GetByIdAsync(request.UseCaseId, cancellationToken);
        if (useCase is null)
            throw new ResourceNotFoundException("UseCase", request.UseCaseId);

        // Modülü kontrol et
        var module = await moduleReadRepository.GetByIdAsync(useCase.ModuleId, cancellationToken);
        if (module is null)
            throw new ResourceNotFoundException("Module", useCase.ModuleId);

        // Projeyi kontrol et
        var project = await projectReadRepository.GetByIdAsync(module.ProjectId, cancellationToken);
        if (project is null)
            throw new ResourceNotFoundException("Project", module.ProjectId);

        // Erişim kontrolü: Kullanıcının projeye erişim yetkisi var mı?
        if (!userPermissionService.CanAccessProject(currentUserId, project))
            throw new ProjectAccessDeniedException(module.ProjectId, currentUserId.Value, "Projeye erişim yetkiniz bulunmamaktadır.");

        var tasks = await taskRepository.GetByUseCaseIdAsync(
            request.UseCaseId,
            request.State,
            request.Type,
            request.AssigneeId,
            request.DueDateFrom,
            request.DueDateTo,
            request.Search,
            cancellationToken);

        var totalCount = tasks.Count;
        var items = new List<TaskListItemResponse>();
        
        foreach (var task in tasks.Skip((request.Page - 1) * request.PageSize).Take(request.PageSize))
        {
            string? assigneeName = null;
            if (task.AssigneeId != null)
            {
                assigneeName = await GetAssigneeNameAsync(task.AssigneeId.Value, cancellationToken);
            }
            
            items.Add(new TaskListItemResponse
            {
                Id = task.Id,
                Title = task.Title,
                TaskType = task.TaskType,
                TaskState = task.TaskState,
                AssigneeId = task.AssigneeId?.Value,
                AssigneeName = assigneeName,
                DueDate = task.DueDate,
                IsOverdue = task.IsOverdue(),
                StartedDate = task.StartedDate
            });
        }

        return new PagedResult<TaskListItemResponse>(items, totalCount, request.Page, request.PageSize);
    }

    private async Task<string?> GetAssigneeNameAsync(Guid assigneeId, CancellationToken cancellationToken)
    {
        var userInfo = await userInfoService.GetUserInfoAsync(assigneeId, cancellationToken);
        return userInfo?.UserName ?? userInfo?.Email;
    }
}

