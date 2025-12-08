using MediatR;
using TE4IT.Abstractions.Persistence.Repositories.Modules;
using TE4IT.Abstractions.Persistence.Repositories.Projects;
using TE4IT.Application.Abstractions.Auth;
using TE4IT.Application.Features.Modules.Responses;
using TE4IT.Domain.Exceptions.Common;
using TE4IT.Domain.Exceptions.Projects;
using TE4IT.Domain.Services;
using TE4IT.Domain.ValueObjects;

namespace TE4IT.Application.Features.Modules.Queries.GetModuleById;

public sealed class GetModuleByIdQueryHandler(
    IModuleReadRepository moduleRepository,
    IProjectReadRepository projectReadRepository,
    ICurrentUser currentUser,
    IUserPermissionService userPermissionService) : IRequestHandler<GetModuleByIdQuery, ModuleResponse>
{
    public async Task<ModuleResponse> Handle(GetModuleByIdQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = currentUser.Id ?? throw new UnauthorizedAccessException();
        
        var module = await moduleRepository.GetByIdAsync(request.ModuleId, cancellationToken);
        if (module is null) throw new ResourceNotFoundException("Module", request.ModuleId);

        // Projeyi kontrol et
        var project = await projectReadRepository.GetByIdAsync(module.ProjectId, cancellationToken);
        if (project is null)
            throw new ResourceNotFoundException("Project", module.ProjectId);

        // Erişim kontrolü: Kullanıcının projeye erişim yetkisi var mı?
        if (!userPermissionService.CanAccessProject(currentUserId, project))
            throw new ProjectAccessDeniedException(module.ProjectId, currentUserId.Value, "Projeye erişim yetkiniz bulunmamaktadır.");

        return new ModuleResponse
        {
            Id = module.Id,
            ProjectId = module.ProjectId,
            Title = module.Title,
            Description = module.Description,
            IsActive = module.IsActive,
            StartedDate = module.StartedDate
        };
    }
}

