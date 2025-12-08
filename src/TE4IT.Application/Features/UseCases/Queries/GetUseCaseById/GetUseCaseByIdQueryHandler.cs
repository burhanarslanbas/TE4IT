using MediatR;
using TE4IT.Abstractions.Persistence.Repositories.Modules;
using TE4IT.Abstractions.Persistence.Repositories.Projects;
using TE4IT.Abstractions.Persistence.Repositories.UseCases;
using TE4IT.Application.Abstractions.Auth;
using TE4IT.Application.Features.UseCases.Responses;
using TE4IT.Domain.Exceptions.Common;
using TE4IT.Domain.Exceptions.Projects;
using TE4IT.Domain.Services;
using TE4IT.Domain.ValueObjects;

namespace TE4IT.Application.Features.UseCases.Queries.GetUseCaseById;

public sealed class GetUseCaseByIdQueryHandler(
    IUseCaseReadRepository useCaseRepository,
    IModuleReadRepository moduleReadRepository,
    IProjectReadRepository projectReadRepository,
    ICurrentUser currentUser,
    IUserPermissionService userPermissionService) : IRequestHandler<GetUseCaseByIdQuery, UseCaseResponse>
{
    public async Task<UseCaseResponse> Handle(GetUseCaseByIdQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = currentUser.Id ?? throw new UnauthorizedAccessException();
        
        var useCase = await useCaseRepository.GetByIdAsync(request.UseCaseId, cancellationToken);
        if (useCase is null) throw new ResourceNotFoundException("UseCase", request.UseCaseId);

        // Modülü kontrol et
        var module = await moduleReadRepository.GetByIdAsync(useCase.ModuleId, cancellationToken);
        if (module is null)
            throw new ResourceNotFoundException("Module", useCase.ModuleId);

        // Projeyi kontrol et
        var project = await projectReadRepository.GetByIdAsync(module.ProjectId, cancellationToken);
        if (project is null)
            throw new ResourceNotFoundException("Project", module.ProjectId);

        // Erişim kontrolü: Kullanıcının projeye erişim yetkisi var mı?
        if (!userPermissionService.CanAccessProject(currentUserId, project))
            throw new ProjectAccessDeniedException(module.ProjectId, currentUserId.Value, "Projeye erişim yetkiniz bulunmamaktadır.");

        return new UseCaseResponse
        {
            Id = useCase.Id,
            ModuleId = useCase.ModuleId,
            Title = useCase.Title,
            Description = useCase.Description,
            ImportantNotes = useCase.ImportantNotes,
            IsActive = useCase.IsActive,
            StartedDate = useCase.StartedDate
        };
    }
}

