using MediatR;
using TE4IT.Abstractions.Persistence.Repositories.ProjectMembers;
using TE4IT.Abstractions.Persistence.Repositories.Projects;
using TE4IT.Application.Abstractions.Auth;
using TE4IT.Application.Abstractions.Persistence;
using TE4IT.Domain.Entities;
using TE4IT.Domain.Enums;
using TE4IT.Domain.Exceptions.Common;
using TE4IT.Domain.Exceptions.Projects;
using TE4IT.Domain.Services;
using TE4IT.Domain.ValueObjects;

namespace TE4IT.Application.Features.Projects.Commands.AddProjectMember;

public sealed class AddProjectMemberCommandHandler(
    IProjectReadRepository projectRepository,
    IProjectMemberReadRepository projectMemberReadRepository,
    IProjectMemberWriteRepository projectMemberWriteRepository,
    IUnitOfWork unitOfWork,
    ICurrentUser currentUser,
    IUserPermissionService userPermissionService) : IRequestHandler<AddProjectMemberCommand, AddProjectMemberCommandResponse>
{
    public async Task<AddProjectMemberCommandResponse> Handle(AddProjectMemberCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = currentUser.Id ?? throw new UnauthorizedAccessException();

        // Projeyi kontrol et
        var project = await projectRepository.GetByIdAsync(request.ProjectId, cancellationToken);
        if (project == null)
            throw new ResourceNotFoundException("Project", request.ProjectId);

        // Yetki kontrolü: Sadece proje sahibi veya admin üye ekleyebilir
        var isAdmin = userPermissionService.IsSystemAdministrator(currentUserId);
        var userRole = userPermissionService.GetUserProjectRole(currentUserId, project);
        
        if (!isAdmin && (!userRole.HasValue || userRole.Value != ProjectRole.Owner))
            throw new ProjectAccessDeniedException(request.ProjectId, currentUserId.Value, "Proje üyesi eklemek için Owner yetkisi gereklidir.");

        // Owner rolü atanamaz (sadece proje oluşturulurken atanır)
        if (request.Role == ProjectRole.Owner)
            throw new BusinessRuleViolationException("Owner rolü sadece proje oluşturulurken atanabilir.");

        // Kullanıcı zaten üye mi kontrol et
        var existingMember = await projectMemberReadRepository.GetByProjectIdAndUserIdAsync(request.ProjectId, request.UserId, cancellationToken);
        if (existingMember != null)
            throw new BusinessRuleViolationException("Kullanıcı zaten bu projenin üyesidir.");

        // ProjectMember oluştur (domain event otomatik eklenir)
        var projectMember = ProjectMember.Create(request.ProjectId, (UserId)request.UserId, request.Role);

        await projectMemberWriteRepository.AddAsync(projectMember, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new AddProjectMemberCommandResponse { ProjectMemberId = projectMember.Id };
    }
}

