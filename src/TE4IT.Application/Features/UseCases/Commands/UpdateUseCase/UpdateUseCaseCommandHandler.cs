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

namespace TE4IT.Application.Features.UseCases.Commands.UpdateUseCase;

public sealed class UpdateUseCaseCommandHandler(
    IUseCaseReadRepository readRepository,
    IUseCaseWriteRepository writeRepository,
    IModuleReadRepository moduleReadRepository,
    IProjectReadRepository projectReadRepository,
    IUnitOfWork unitOfWork,
    ICurrentUser currentUser,
    IUserPermissionService userPermissionService) : IRequestHandler<UpdateUseCaseCommand, bool>
{
    public async Task<bool> Handle(UpdateUseCaseCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = currentUser.Id ?? throw new UnauthorizedAccessException();
        
        var useCase = await readRepository.GetByIdAsync(request.UseCaseId, cancellationToken);
        if (useCase is null) return false;

        // Modülün aktif olduğunu kontrol et
        var module = await moduleReadRepository.GetByIdAsync(useCase.ModuleId, cancellationToken);
        if (module is null)
            throw new ResourceNotFoundException("Modül bulunamadı.");

        // Projeyi kontrol et
        var project = await projectReadRepository.GetByIdAsync(module.ProjectId, cancellationToken);
        if (project is null)
            throw new ResourceNotFoundException("Proje bulunamadı.");

        // Erişim kontrolü: Kullanıcının projeyi düzenleme yetkisi var mı?
        if (!userPermissionService.CanEditProject(currentUserId, project))
            throw new ProjectAccessDeniedException(module.ProjectId, currentUserId.Value, "Projede kullanım senaryosu düzenleme yetkiniz bulunmamaktadır.");

        if (!module.IsActive)
            throw new BusinessRuleViolationException("Arşivlenmiş modülde kullanım senaryosu güncellenemez.");

        useCase.UpdateTitle(request.Title);
        useCase.UpdateDescription(request.Description);
        useCase.UpdateImportantNotes(request.ImportantNotes);

        writeRepository.Update(useCase, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}

