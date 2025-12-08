using MediatR;
using TE4IT.Abstractions.Persistence.Repositories.Projects;
using TE4IT.Application.Abstractions.Auth;
using TE4IT.Application.Abstractions.Persistence;
using TE4IT.Domain.Exceptions.Projects;
using TE4IT.Domain.Services;
using TE4IT.Domain.ValueObjects;

namespace TE4IT.Application.Features.Projects.Commands.DeleteProject;

public sealed class DeleteProjectCommandHandler(
    IProjectReadRepository readRepository,
    IProjectWriteRepository writeRepository,
    IUnitOfWork unitOfWork,
    ICurrentUser currentUser,
    IUserPermissionService userPermissionService) : IRequestHandler<DeleteProjectCommand, bool>
{
    public async Task<bool> Handle(DeleteProjectCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = currentUser.Id ?? throw new UnauthorizedAccessException();
        
        var project = await readRepository.GetByIdAsync(request.ProjectId, cancellationToken);
        if (project is null) return false;

        // Erişim kontrolü: Kullanıcının projeyi silme yetkisi var mı? (Sadece Owner veya Admin)
        if (!userPermissionService.CanDeleteProject(currentUserId, project))
            throw new ProjectAccessDeniedException(request.ProjectId, currentUserId.Value, "Projeyi silme yetkiniz bulunmamaktadır.");

        writeRepository.Remove(project, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}


