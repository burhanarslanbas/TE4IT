using MediatR;
using TE4IT.Abstractions.Persistence.Repositories.Modules;
using TE4IT.Abstractions.Persistence.Repositories.Projects;
using TE4IT.Abstractions.Persistence.Repositories.UseCases;
using TE4IT.Application.Abstractions.Auth;
using TE4IT.Application.Abstractions.Persistence;
using TE4IT.Domain.Entities;
using TE4IT.Domain.Exceptions.Common;
using TE4IT.Domain.Exceptions.Projects;
using TE4IT.Domain.Services;
using TE4IT.Domain.ValueObjects;

namespace TE4IT.Application.Features.UseCases.Commands.CreateUseCase;

public sealed class CreateUseCaseCommandHandler(
    IUseCaseWriteRepository useCaseRepository,
    IModuleReadRepository moduleReadRepository,
    IProjectReadRepository projectReadRepository,
    IUnitOfWork unitOfWork,
    ICurrentUser currentUser,
    IUserPermissionService userPermissionService) : IRequestHandler<CreateUseCaseCommand, CreateUseCaseCommandResponse>
{
    public async Task<CreateUseCaseCommandResponse> Handle(CreateUseCaseCommand request, CancellationToken cancellationToken)
    {
        var creatorId = currentUser.Id ?? throw new UnauthorizedAccessException();

        // Modülün aktif olduğunu kontrol et
        var module = await moduleReadRepository.GetByIdAsync(request.ModuleId, cancellationToken);
        if (module is null)
            throw new ResourceNotFoundException("Modül bulunamadı.");

        // Projeyi kontrol et
        var project = await projectReadRepository.GetByIdAsync(module.ProjectId, cancellationToken);
        if (project is null)
            throw new ResourceNotFoundException("Proje bulunamadı.");

        // Erişim kontrolü: Kullanıcının projede use case oluşturma yetkisi var mı?
        if (!userPermissionService.CanCreateUseCase(creatorId, module, project))
            throw new ProjectAccessDeniedException(module.ProjectId, creatorId.Value, "Projede kullanım senaryosu oluşturma yetkiniz bulunmamaktadır.");

        if (!module.IsActive)
            throw new BusinessRuleViolationException("Arşivlenmiş modülde kullanım senaryosu oluşturulamaz.");

        var useCase = UseCase.Create(request.ModuleId, creatorId, request.Title, request.Description, request.ImportantNotes);
        await useCaseRepository.AddAsync(useCase, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreateUseCaseCommandResponse { Id = useCase.Id };
    }
}

