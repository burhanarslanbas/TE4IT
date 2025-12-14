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

namespace TE4IT.Application.Features.Modules.Commands.DeleteModule;

public sealed class DeleteModuleCommandHandler(
    IModuleReadRepository readRepository,
    IModuleWriteRepository writeRepository,
    IUseCaseReadRepository useCaseRepository,
    IProjectReadRepository projectReadRepository,
    IUnitOfWork unitOfWork,
    ICurrentUser currentUser,
    IUserPermissionService userPermissionService) : IRequestHandler<DeleteModuleCommand, bool>
{
    public async Task<bool> Handle(DeleteModuleCommand request, CancellationToken cancellationToken)
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
            throw new ProjectAccessDeniedException(module.ProjectId, currentUserId.Value, "Projede modül silme yetkiniz bulunmamaktadır.");

        // UseCase kontrolü: İçinde UseCase varsa uyarı ver
        var useCaseCount = await useCaseRepository.CountByModuleAsync(module.Id, cancellationToken);
        if (useCaseCount > 0)
        {
            throw new BusinessRuleViolationException(
                $"Modül içinde {useCaseCount} adet UseCase bulunmaktadır. " +
                "Modül silindiğinde tüm UseCase'ler de silinecektir.");
        }

        writeRepository.Remove(module, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}

