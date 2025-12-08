using MediatR;
using TE4IT.Abstractions.Persistence.Repositories.ProjectMembers;
using TE4IT.Abstractions.Persistence.Repositories.Projects;
using TE4IT.Application.Abstractions.Auth;
using TE4IT.Application.Abstractions.Persistence;
using TE4IT.Domain.Enums;
using TE4IT.Domain.Exceptions.Common;
using TE4IT.Domain.Exceptions.Projects;
using TE4IT.Domain.Services;
using TE4IT.Domain.ValueObjects;

namespace TE4IT.Application.Features.Projects.Commands.RemoveProjectMember;

public sealed class RemoveProjectMemberCommandHandler(
    IProjectReadRepository projectRepository,
    IProjectMemberReadRepository projectMemberReadRepository,
    IProjectMemberWriteRepository projectMemberWriteRepository,
    IUnitOfWork unitOfWork,
    ICurrentUser currentUser,
    IUserPermissionService userPermissionService) : IRequestHandler<RemoveProjectMemberCommand, bool>
{
    public async Task<bool> Handle(RemoveProjectMemberCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = currentUser.Id ?? throw new UnauthorizedAccessException();

        // Projeyi kontrol et
        var project = await projectRepository.GetByIdAsync(request.ProjectId, cancellationToken);
        if (project == null)
            throw new ResourceNotFoundException("Project", request.ProjectId);

        // Yetki kontrolü: Sadece proje sahibi veya admin üye çıkarabilir
        var isAdmin = userPermissionService.IsSystemAdministrator(currentUserId);
        var userRole = userPermissionService.GetUserProjectRole(currentUserId, project);
        
        if (!isAdmin && (!userRole.HasValue || userRole.Value != ProjectRole.Owner))
            throw new ProjectAccessDeniedException(request.ProjectId, currentUserId.Value, "Proje üyesi çıkarmak için Owner yetkisi gereklidir.");

        // Üyeyi bul
        var member = await projectMemberReadRepository.GetByProjectIdAndUserIdAsync(request.ProjectId, request.UserId, cancellationToken);
        if (member == null)
            throw new ResourceNotFoundException("ProjectMember", request.UserId);

        // Owner (proje sahibi) çıkarılamaz
        if (member.Role == ProjectRole.Owner)
            throw new BusinessRuleViolationException("Proje sahibi (Owner) çıkarılamaz.");

        // Kullanıcı kendini çıkaramaz
        if (request.UserId == currentUserId.Value)
            throw new BusinessRuleViolationException("Kullanıcı kendini projeden çıkaramaz.");

        // Domain event ekle ve üyeyi çıkar
        member.Remove();
        projectMemberWriteRepository.Remove(member);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}

