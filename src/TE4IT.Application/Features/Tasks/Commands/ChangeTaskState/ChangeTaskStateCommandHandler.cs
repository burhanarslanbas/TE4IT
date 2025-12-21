using MediatR;
using TE4IT.Abstractions.Persistence.Repositories.Modules;
using TE4IT.Abstractions.Persistence.Repositories.Projects;
using TE4IT.Abstractions.Persistence.Repositories.Tasks;
using TE4IT.Abstractions.Persistence.Repositories.UseCases;
using TE4IT.Application.Abstractions.Auth;
using TE4IT.Application.Abstractions.Persistence;
using TaskEntity = TE4IT.Domain.Entities.Task;
using TE4IT.Domain.Enums;
using TE4IT.Domain.Exceptions.Common;
using TE4IT.Domain.Exceptions.Projects;
using TE4IT.Domain.Services;
using TE4IT.Domain.ValueObjects;

namespace TE4IT.Application.Features.Tasks.Commands.ChangeTaskState;

public sealed class ChangeTaskStateCommandHandler(
    ITaskReadRepository readRepository,
    ITaskWriteRepository writeRepository,
    IUseCaseReadRepository useCaseReadRepository,
    IModuleReadRepository moduleReadRepository,
    IProjectReadRepository projectReadRepository,
    IUnitOfWork unitOfWork,
    ICurrentUser currentUser,
    IUserPermissionService userPermissionService) : IRequestHandler<ChangeTaskStateCommand, bool>
{
    public async Task<bool> Handle(ChangeTaskStateCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = currentUser.Id ?? throw new UnauthorizedAccessException();
        
        var task = await readRepository.GetByIdAsync(request.TaskId, cancellationToken);
        if (task is null) return false;

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

        // Erişim kontrolü: Kullanıcının görevi düzenleme yetkisi var mı?
        if (!userPermissionService.CanEditTask(currentUserId, task, project))
            throw new ProjectAccessDeniedException(module.ProjectId, currentUserId.Value, "Görev durumunu değiştirme yetkiniz bulunmamaktadır.");

        switch (request.NewState)
        {
            case TaskState.InProgress:
                task.Start();
                break;
            case TaskState.Completed:
                if (!task.CanBeCompleted())
                    throw new Domain.Exceptions.Common.BusinessRuleViolationException("Görev tamamlanamaz. Bloklayan bağımlılıklar var veya görev atanmamış.");
                task.Complete(); // CompletionNote olmadan tamamla (eski API uyumluluğu için)
                break;
            case TaskState.Cancelled:
                task.Cancel();
                break;
            case TaskState.NotStarted:
                task.Revert();
                break;
            default:
                throw new ArgumentException("Geçersiz görev durumu.");
        }

        writeRepository.Update(task, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}

