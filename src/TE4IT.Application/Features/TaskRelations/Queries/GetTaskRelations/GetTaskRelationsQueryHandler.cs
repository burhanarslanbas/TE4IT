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

namespace TE4IT.Application.Features.TaskRelations.Queries.GetTaskRelations;

public sealed class GetTaskRelationsQueryHandler(
    ITaskReadRepository taskRepository,
    IUseCaseReadRepository useCaseReadRepository,
    IModuleReadRepository moduleReadRepository,
    IProjectReadRepository projectReadRepository,
    ICurrentUser currentUser,
    IUserPermissionService userPermissionService) : IRequestHandler<GetTaskRelationsQuery, List<TaskRelationResponse>>
{
    public async Task<List<TaskRelationResponse>> Handle(GetTaskRelationsQuery request, CancellationToken cancellationToken)
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
            throw new ProjectAccessDeniedException(module.ProjectId, currentUserId.Value, "Görev ilişkilerini görüntüleme yetkiniz bulunmamaktadır.");

        return task.Relations.Select(r => new TaskRelationResponse
        {
            Id = r.Id,
            TargetTaskId = r.TargetTaskId,
            RelationType = r.RelationType,
            TargetTaskTitle = string.Empty // Will be populated when repository extension is added
        }).ToList();
    }
}

