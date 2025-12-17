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

namespace TE4IT.Application.Features.Tasks.Commands.UpdateTask;

public sealed class UpdateTaskCommandHandler(
    ITaskReadRepository readRepository,
    ITaskWriteRepository writeRepository,
    IUseCaseReadRepository useCaseReadRepository,
    IModuleReadRepository moduleReadRepository,
    IProjectReadRepository projectReadRepository,
    IUnitOfWork unitOfWork,
    ICurrentUser currentUser,
    IUserPermissionService userPermissionService) : IRequestHandler<UpdateTaskCommand, bool>
{
    public async Task<bool> Handle(UpdateTaskCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = currentUser.Id ?? throw new UnauthorizedAccessException();
        
        var task = await readRepository.GetByIdAsync(request.TaskId, cancellationToken);
        if (task is null) return false;

        // Kullanım senaryosunun aktif olduğunu kontrol et
        var useCase = await useCaseReadRepository.GetByIdAsync(task.UseCaseId, cancellationToken);
        if (useCase is null)
            throw new ResourceNotFoundException("Kullanım senaryosu bulunamadı.");

        var module = await moduleReadRepository.GetByIdAsync(useCase.ModuleId, cancellationToken);
        if (module is null)
            throw new ResourceNotFoundException("Modül bulunamadı.");

        var project = await projectReadRepository.GetByIdAsync(module.ProjectId, cancellationToken);
        if (project is null)
            throw new ResourceNotFoundException("Proje bulunamadı.");

        // Erişim kontrolü: Kullanıcının görevi düzenleme yetkisi var mı?
        if (!userPermissionService.CanEditTask(currentUserId, task, project))
            throw new ProjectAccessDeniedException(module.ProjectId, currentUserId.Value, "Görevi düzenleme yetkiniz bulunmamaktadır.");

        if (!useCase.IsActive)
            throw new BusinessRuleViolationException("Arşivlenmiş kullanım senaryosunda görev güncellenemez.");

        task.UpdateTitle(request.Title);
        task.UpdateDescription(request.Description);
        task.UpdateImportantNotes(request.ImportantNotes);
        task.UpdateDueDate(request.DueDate);

        writeRepository.Update(task, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}

