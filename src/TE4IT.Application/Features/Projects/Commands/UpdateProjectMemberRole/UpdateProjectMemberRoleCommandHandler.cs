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

namespace TE4IT.Application.Features.Projects.Commands.UpdateProjectMemberRole;

public sealed class UpdateProjectMemberRoleCommandHandler(
    IProjectReadRepository projectRepository,
    IProjectMemberReadRepository projectMemberReadRepository,
    IProjectMemberWriteRepository projectMemberWriteRepository,
    IUnitOfWork unitOfWork,
    ICurrentUser currentUser,
    IUserPermissionService userPermissionService) : IRequestHandler<UpdateProjectMemberRoleCommand, bool>
{
    public async Task<bool> Handle(UpdateProjectMemberRoleCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = currentUser.Id ?? throw new UnauthorizedAccessException();

        // Projeyi kontrol et
        var project = await projectRepository.GetByIdAsync(request.ProjectId, cancellationToken);
        if (project == null)
            throw new ResourceNotFoundException("Project", request.ProjectId);

        // Yetki kontrolü: Sadece proje sahibi veya admin rol güncelleyebilir
        var isAdmin = userPermissionService.IsSystemAdministrator(currentUserId);
        var userRole = userPermissionService.GetUserProjectRole(currentUserId, project);
        
        if (!isAdmin && (!userRole.HasValue || userRole.Value != ProjectRole.Owner))
            throw new ProjectAccessDeniedException(request.ProjectId, currentUserId.Value, "Owner permission is required to update project member roles.");

        // Üyeyi bul
        var member = await projectMemberReadRepository.GetByProjectIdAndUserIdAsync(request.ProjectId, request.UserId, cancellationToken);
        if (member == null)
            throw new ResourceNotFoundException("ProjectMember", request.UserId);

        // Eğer Owner'ın rolü düşürülmeye çalışılıyorsa, son Owner kontrolü yap
        if (member.Role == ProjectRole.Owner && request.Role != ProjectRole.Owner)
        {
            var ownerCount = await projectMemberReadRepository.CountByProjectIdAndRoleAsync(
                request.ProjectId, 
                ProjectRole.Owner, 
                cancellationToken);
            
            if (ownerCount <= 1)
                throw new BusinessRuleViolationException("The last Owner's role cannot be changed. At least one Owner must exist.");
        }

        // Owner rolü atanamaz (güvenlik için)
        if (request.Role == ProjectRole.Owner)
            throw new BusinessRuleViolationException("Owner role cannot be assigned through this endpoint. Owner permission can only be granted during project creation or through a special process.");

        // Rolü güncelle (domain event otomatik eklenir)
        member.UpdateRole(request.Role);

        projectMemberWriteRepository.Update(member);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}

