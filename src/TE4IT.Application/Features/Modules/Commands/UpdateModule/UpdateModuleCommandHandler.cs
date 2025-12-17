using MediatR;
using TE4IT.Abstractions.Persistence.Repositories.Modules;
using TE4IT.Abstractions.Persistence.Repositories.Projects;
using TE4IT.Application.Abstractions.Auth;
using TE4IT.Application.Abstractions.Persistence;
using TE4IT.Domain.Exceptions.Common;
using TE4IT.Domain.Exceptions.Projects;
using TE4IT.Domain.Services;
using TE4IT.Domain.ValueObjects;

namespace TE4IT.Application.Features.Modules.Commands.UpdateModule;

public sealed class UpdateModuleCommandHandler(
    IModuleReadRepository readRepository,
    IModuleWriteRepository writeRepository,
    IProjectReadRepository projectReadRepository,
    IUnitOfWork unitOfWork,
    ICurrentUser currentUser,
    IUserPermissionService userPermissionService) : IRequestHandler<UpdateModuleCommand, bool>
{
    public async Task<bool> Handle(UpdateModuleCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = currentUser.Id ?? throw new UnauthorizedAccessException();
        
        var module = await readRepository.GetByIdAsync(request.ModuleId, cancellationToken);
        if (module is null) return false;

        // Projenin aktif olduğunu kontrol et
        var project = await projectReadRepository.GetByIdAsync(module.ProjectId, cancellationToken);
        if (project is null)
            throw new ResourceNotFoundException("Proje bulunamadı.");

        // Erişim kontrolü: Kullanıcının projeyi düzenleme yetkisi var mı?
        if (!userPermissionService.CanEditProject(currentUserId, project))
            throw new ProjectAccessDeniedException(module.ProjectId, currentUserId.Value, "Projede modül düzenleme yetkiniz bulunmamaktadır.");

        if (!project.IsActive)
            throw new BusinessRuleViolationException("Arşivlenmiş projede modül güncellenemez.");

        module.UpdateTitle(request.Title);
        module.UpdateDescription(request.Description);

        writeRepository.Update(module, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}

