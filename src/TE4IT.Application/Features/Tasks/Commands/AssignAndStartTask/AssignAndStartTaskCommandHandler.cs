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

namespace TE4IT.Application.Features.Tasks.Commands.AssignAndStartTask;

public sealed class AssignAndStartTaskCommandHandler(
    ITaskReadRepository readRepository,
    ITaskWriteRepository writeRepository,
    IUseCaseReadRepository useCaseReadRepository,
    IModuleReadRepository moduleReadRepository,
    IProjectReadRepository projectReadRepository,
    IUnitOfWork unitOfWork,
    ICurrentUser currentUser,
    IUserPermissionService userPermissionService,
    IUserInfoService userInfoService) : IRequestHandler<AssignAndStartTaskCommand, bool>
{
    public async Task<bool> Handle(AssignAndStartTaskCommand request, CancellationToken cancellationToken)
    {
        var assignerId = currentUser.Id ?? throw new UnauthorizedAccessException();

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

        // Erişim kontrolü: Kullanıcının görevi atama yetkisi var mı?
        if (!userPermissionService.CanAssignTask(assignerId, task, project))
            throw new ProjectAccessDeniedException(module.ProjectId, assignerId.Value, "Görevi atama yetkiniz bulunmamaktadır.");

        // Atanan kişinin var olup olmadığını kontrol et
        var assigneeId = (UserId)request.AssigneeId;
        var assigneeInfo = await userInfoService.GetUserInfoAsync(assigneeId.Value, cancellationToken);
        if (assigneeInfo == null)
            throw new ResourceNotFoundException("Kullanıcı", assigneeId.Value);

        // Atanan kişinin projeye erişim yetkisi var mı kontrol et
        if (!userPermissionService.CanAccessProject(assigneeId, project))
            throw new ProjectAccessDeniedException(module.ProjectId, assigneeId.Value, "Atanan kişinin projeye erişim yetkisi bulunmamaktadır.");

        task.AssignTo(assigneeId, assignerId);
        task.Start();

        writeRepository.Update(task, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}

