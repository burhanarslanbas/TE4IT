using System.Threading.Tasks;
using MediatR;
using TE4IT.Abstractions.Persistence.Repositories.Modules;
using TE4IT.Abstractions.Persistence.Repositories.Projects;
using TE4IT.Abstractions.Persistence.Repositories.Tasks;
using TE4IT.Abstractions.Persistence.Repositories.UseCases;
using TE4IT.Application.Abstractions.Auth;
using TE4IT.Application.Abstractions.Persistence;
using TaskEntity = TE4IT.Domain.Entities.Task;
using TE4IT.Domain.Exceptions.Common;
using TE4IT.Domain.Exceptions.Projects;
using TE4IT.Domain.Services;
using TE4IT.Domain.ValueObjects;

namespace TE4IT.Application.Features.Tasks.Commands.CreateTask;

public sealed class CreateTaskCommandHandler(
    ITaskWriteRepository taskRepository,
    IUseCaseReadRepository useCaseReadRepository,
    IModuleReadRepository moduleReadRepository,
    IProjectReadRepository projectReadRepository,
    IUnitOfWork unitOfWork,
    ICurrentUser currentUser,
    IUserPermissionService userPermissionService,
    IUserInfoService userInfoService) : IRequestHandler<CreateTaskCommand, CreateTaskCommandResponse>
{
    public async Task<CreateTaskCommandResponse> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
    {
        var creatorId = currentUser.Id ?? throw new UnauthorizedAccessException();

        // Kullanım senaryosunun aktif olduğunu kontrol et
        var useCase = await useCaseReadRepository.GetByIdAsync(request.UseCaseId, cancellationToken);
        if (useCase is null)
            throw new ResourceNotFoundException("Kullanım senaryosu bulunamadı.");

        // Modülü kontrol et
        var module = await moduleReadRepository.GetByIdAsync(useCase.ModuleId, cancellationToken);
        if (module is null)
            throw new ResourceNotFoundException("Modül bulunamadı.");

        // Projeyi kontrol et
        var project = await projectReadRepository.GetByIdAsync(module.ProjectId, cancellationToken);
        if (project is null)
            throw new ResourceNotFoundException("Proje bulunamadı.");

        // Erişim kontrolü: Kullanıcının projede task oluşturma yetkisi var mı?
        if (!userPermissionService.CanCreateTask(creatorId, useCase, project))
            throw new ProjectAccessDeniedException(module.ProjectId, creatorId.Value, "Projede görev oluşturma yetkiniz bulunmamaktadır.");

        if (!useCase.IsActive)
            throw new BusinessRuleViolationException("Arşivlenmiş kullanım senaryosunda görev oluşturulamaz.");

        var task = TaskEntity.Create(request.UseCaseId, creatorId, request.Title, request.TaskType, request.Description, request.ImportantNotes);
        
        if (request.DueDate.HasValue)
            task.UpdateDueDate(request.DueDate.Value);

        // Eğer AssigneeId verilmişse, atama yap
        if (request.AssigneeId.HasValue)
        {
            var assigneeId = (UserId)request.AssigneeId.Value;
            
            // Atanan kişinin var olup olmadığını kontrol et
            var assigneeInfo = await userInfoService.GetUserInfoAsync(assigneeId.Value, cancellationToken);
            if (assigneeInfo == null)
                throw new ResourceNotFoundException("Kullanıcı", assigneeId.Value);
            
            // Atanan kişinin projeye erişim yetkisi var mı kontrol et
            if (!userPermissionService.CanAccessProject(assigneeId, project))
                throw new ProjectAccessDeniedException(module.ProjectId, assigneeId.Value, "Atanan kişinin projeye erişim yetkisi bulunmamaktadır.");

            // Atama yetkisi kontrolü: Kullanıcının görevi atama yetkisi var mı?
            // Not: Task henüz oluşturulmadığı için CanAssignTask kullanamayız, 
            // bunun yerine proje rolüne göre kontrol yapıyoruz
            if (!userPermissionService.CanEditProject(creatorId, project))
                throw new ProjectAccessDeniedException(module.ProjectId, creatorId.Value, "Görevi atama yetkiniz bulunmamaktadır.");

            task.AssignTo(assigneeId, creatorId);
        }

        await taskRepository.AddAsync(task, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreateTaskCommandResponse { Id = task.Id };
    }
}

