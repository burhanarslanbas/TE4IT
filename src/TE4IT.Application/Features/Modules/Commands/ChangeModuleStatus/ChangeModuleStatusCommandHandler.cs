using MediatR;
using TE4IT.Abstractions.Persistence.Repositories.Modules;
using TE4IT.Abstractions.Persistence.Repositories.Projects;
using TE4IT.Abstractions.Persistence.Repositories.UseCases;
using TE4IT.Application.Abstractions.Auth;
using TE4IT.Application.Abstractions.Persistence;
using TE4IT.Domain.Exceptions.Common;
using TE4IT.Domain.Exceptions.Projects;
using TE4IT.Domain.Services;
using TE4IT.Domain.ValueObjects;

namespace TE4IT.Application.Features.Modules.Commands.ChangeModuleStatus;

public sealed class ChangeModuleStatusCommandHandler(
    IModuleReadRepository readRepository,
    IModuleWriteRepository writeRepository,
    IUseCaseWriteRepository useCaseWriteRepository,
    IProjectReadRepository projectReadRepository,
    IUnitOfWork unitOfWork,
    ICurrentUser currentUser,
    IUserPermissionService userPermissionService) : IRequestHandler<ChangeModuleStatusCommand, bool>
{
    public async Task<bool> Handle(ChangeModuleStatusCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = currentUser.Id ?? throw new UnauthorizedAccessException();
        
        var module = await readRepository.GetByIdAsync(request.ModuleId, cancellationToken);
        if (module is null) return false;

        // Projeyi kontrol et
        var project = await projectReadRepository.GetByIdAsync(module.ProjectId, cancellationToken);
        if (project is null)
            throw new ResourceNotFoundException("Proje bulunamadı.");

        // Erişim kontrolü: Kullanıcının projeyi düzenleme yetkisi var mı?
        if (!userPermissionService.CanEditProject(currentUserId, project))
            throw new ProjectAccessDeniedException(module.ProjectId, currentUserId.Value, "Projede modül durumu değiştirme yetkiniz bulunmamaktadır.");

        // Arşivlenmiş projede modül aktif edilemez kontrolü
        if (!project.IsActive && request.IsActive)
        {
            throw new BusinessRuleViolationException(
                "Arşivlenmiş projede modül aktif edilemez. Önce projeyi aktif edin.");
        }

        if (request.IsActive)
        {
            module.Activate();
        }
        else
        {
            module.Archive();
            // Cascade Archive: Module arşivlendiğinde içindeki tüm UseCase'leri de arşivle
            await useCaseWriteRepository.ArchiveByModuleIdAsync(module.Id, cancellationToken);
        }

        writeRepository.Update(module, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}

