using MediatR;
using TE4IT.Abstractions.Persistence.Repositories.Modules;
using TE4IT.Abstractions.Persistence.Repositories.Projects;
using TE4IT.Abstractions.Persistence.Repositories.Tasks;
using TE4IT.Abstractions.Persistence.Repositories.UseCases;
using TE4IT.Application.Abstractions.Auth;
using TE4IT.Application.Features.Tasks.Responses;
using TaskEntity = TE4IT.Domain.Entities.Task;
using TE4IT.Domain.Exceptions.Common;
using TE4IT.Domain.Exceptions.Projects;
using TE4IT.Domain.Services;
using TE4IT.Domain.ValueObjects;

namespace TE4IT.Application.Features.Tasks.Queries.GetTaskById;

public sealed class GetTaskByIdQueryHandler(
    ITaskReadRepository taskRepository,
    IUseCaseReadRepository useCaseReadRepository,
    IModuleReadRepository moduleReadRepository,
    IProjectReadRepository projectReadRepository,
    IUserInfoService userInfoService,
    ICurrentUser currentUser,
    IUserPermissionService userPermissionService) : IRequestHandler<GetTaskByIdQuery, TaskResponse>
{
    public async Task<TaskResponse> Handle(GetTaskByIdQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = currentUser.Id ?? throw new UnauthorizedAccessException();
        
        var task = await taskRepository.GetByIdWithRelationsAsync(request.TaskId, cancellationToken);
        if (task is null) throw new ResourceNotFoundException("Task", request.TaskId);

        // Proje bilgisini al (UseCase -> Module -> Project)
        var useCase = await useCaseReadRepository.GetByIdAsync(task.UseCaseId, cancellationToken);
        if (useCase is null)
            throw new ResourceNotFoundException("Kullanım senaryosu bulunamadı.");

        var module = await moduleReadRepository.GetByIdAsync(useCase.ModuleId, cancellationToken);
        if (module is null)
            throw new ResourceNotFoundException("Modül bulunamadı.");

        var project = await projectReadRepository.GetByIdAsync(module.ProjectId, cancellationToken);
        if (project is null)
            throw new ResourceNotFoundException("Proje bulunamadı.");

        // Erişim kontrolü: Kullanıcının görevi görüntüleme yetkisi var mı?
        if (!userPermissionService.CanViewTask(currentUserId, task, project))
            throw new ProjectAccessDeniedException(module.ProjectId, currentUserId.Value, "Görevi görüntüleme yetkiniz bulunmamaktadır.");

        string? assigneeName = null;
        if (task.AssigneeId != null)
        {
            assigneeName = await GetAssigneeNameAsync(task.AssigneeId.Value, cancellationToken);
        }
        
        var relations = new List<TaskRelationResponse>();
        foreach (var relation in task.Relations)
        {
            var targetTask = await taskRepository.GetByIdAsync(relation.TargetTaskId, cancellationToken);
            relations.Add(new TaskRelationResponse
            {
                Id = relation.Id,
                TargetTaskId = relation.TargetTaskId,
                RelationType = relation.RelationType,
                TargetTaskTitle = targetTask?.Title ?? string.Empty
            });
        }

        return new TaskResponse
        {
            Id = task.Id,
            UseCaseId = task.UseCaseId,
            CreatorId = task.CreatorId.Value,
            AssigneeId = task.AssigneeId?.Value,
            AssigneeName = assigneeName,
            Title = task.Title,
            Description = task.Description,
            ImportantNotes = task.ImportantNotes,
            StartedDate = task.StartedDate,
            DueDate = task.DueDate,
            TaskType = task.TaskType,
            TaskState = task.TaskState,
            Relations = relations
        };
    }

    private async Task<string?> GetAssigneeNameAsync(Guid assigneeId, CancellationToken cancellationToken)
    {
        var userInfo = await userInfoService.GetUserInfoAsync(assigneeId, cancellationToken);
        return userInfo?.UserName ?? userInfo?.Email;
    }
}

