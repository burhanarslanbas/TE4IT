using MediatR;
using TE4IT.Abstractions.Persistence.Repositories.Projects;
using TE4IT.Application.Abstractions.Auth;
using TE4IT.Application.Abstractions.Persistence;
using TE4IT.Domain.Exceptions.Projects;
using TE4IT.Domain.Services;
using TE4IT.Domain.ValueObjects;

namespace TE4IT.Application.Features.Projects.Commands.UpdateProject;

public sealed class UpdateProjectCommandHandler(
    IProjectReadRepository readRepository,
    IProjectWriteRepository writeRepository,
    IUnitOfWork unitOfWork,
    ICurrentUser currentUser,
    IUserPermissionService userPermissionService) : IRequestHandler<UpdateProjectCommand, bool>
{
    public async Task<bool> Handle(UpdateProjectCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = currentUser.Id ?? throw new UnauthorizedAccessException();
        
        var project = await readRepository.GetByIdAsync(request.ProjectId, cancellationToken);
        if (project is null) return false;

        // Erişim kontrolü: Kullanıcının projeyi düzenleme yetkisi var mı?
        if (!userPermissionService.CanEditProject(currentUserId, project))
            throw new ProjectAccessDeniedException(request.ProjectId, currentUserId.Value, "Projeyi düzenleme yetkiniz bulunmamaktadır.");

        project.UpdateTitle(request.Title);
        project.UpdateDescription(request.Description);

        writeRepository.Update(project, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}


