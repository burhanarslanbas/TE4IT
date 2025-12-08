using MediatR;
using TE4IT.Abstractions.Persistence.Repositories.Modules;
using TE4IT.Abstractions.Persistence.Repositories.Projects;
using TE4IT.Application.Abstractions.Auth;
using TE4IT.Application.Abstractions.Persistence;
using TE4IT.Domain.Entities;
using TE4IT.Domain.Exceptions.Common;
using TE4IT.Domain.Exceptions.Projects;
using TE4IT.Domain.Services;
using TE4IT.Domain.ValueObjects;

namespace TE4IT.Application.Features.Modules.Commands.CreateModule;

public sealed class CreateModuleCommandHandler(
    IModuleWriteRepository moduleRepository,
    IProjectReadRepository projectReadRepository,
    IUnitOfWork unitOfWork,
    ICurrentUser currentUser,
    IUserPermissionService userPermissionService) : IRequestHandler<CreateModuleCommand, CreateModuleCommandResponse>
{
    public async Task<CreateModuleCommandResponse> Handle(CreateModuleCommand request, CancellationToken cancellationToken)
    {
        var creatorId = currentUser.Id ?? throw new UnauthorizedAccessException();

        // Projenin aktif olduğunu kontrol et
        var project = await projectReadRepository.GetByIdAsync(request.ProjectId, cancellationToken);
        if (project is null)
            throw new ResourceNotFoundException("Proje bulunamadı.");

        // Erişim kontrolü: Kullanıcının projede modül oluşturma yetkisi var mı?
        if (!userPermissionService.CanCreateModule(creatorId, project))
            throw new ProjectAccessDeniedException(request.ProjectId, creatorId.Value, "Projede modül oluşturma yetkiniz bulunmamaktadır.");

        if (!project.IsActive)
            throw new BusinessRuleViolationException("Arşivlenmiş projede modül oluşturulamaz.");

        var module = Module.Create(request.ProjectId, creatorId, request.Title, request.Description);
        await moduleRepository.AddAsync(module, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreateModuleCommandResponse { Id = module.Id };
    }
}

