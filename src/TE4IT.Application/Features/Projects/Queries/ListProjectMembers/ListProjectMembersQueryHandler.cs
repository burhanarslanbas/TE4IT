using MediatR;
using TE4IT.Abstractions.Persistence.Repositories.ProjectMembers;
using TE4IT.Abstractions.Persistence.Repositories.Projects;
using TE4IT.Application.Abstractions.Auth;
using TE4IT.Application.Features.Projects.Responses;
using TE4IT.Domain.Enums;
using TE4IT.Domain.Exceptions.Common;
using TE4IT.Domain.Exceptions.Projects;
using TE4IT.Domain.Services;
using TE4IT.Domain.ValueObjects;

namespace TE4IT.Application.Features.Projects.Queries.ListProjectMembers;

public sealed class ListProjectMembersQueryHandler(
    IProjectReadRepository projectRepository,
    IProjectMemberReadRepository projectMemberReadRepository,
    IUserInfoService userInfoService,
    ICurrentUser currentUser,
    IUserPermissionService userPermissionService) : IRequestHandler<ListProjectMembersQuery, List<ProjectMemberResponse>>
{
    public async Task<List<ProjectMemberResponse>> Handle(ListProjectMembersQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = currentUser.Id ?? throw new UnauthorizedAccessException();

        // Projeyi kontrol et
        var project = await projectRepository.GetByIdAsync(request.ProjectId, cancellationToken);
        if (project == null)
            throw new ResourceNotFoundException("Project", request.ProjectId);

        // Erişim kontrolü: Projeye erişim yetkisi olanlar üyeleri görebilir
        var isAdmin = userPermissionService.IsSystemAdministrator(currentUserId);
        if (!isAdmin && !userPermissionService.CanAccessProject(currentUserId, project))
            throw new ProjectAccessDeniedException(request.ProjectId, currentUserId.Value, "Proje üyelerini görüntülemek için erişim yetkisi gereklidir.");

        // Üyeleri getir
        var members = await projectMemberReadRepository.GetByProjectIdAsync(request.ProjectId, cancellationToken);

        // Proje sahibini de ekle (eğer ProjectMembers tablosunda yoksa)
        var ownerInMembers = members.Any(m => m.Role == ProjectRole.Owner);
        if (!ownerInMembers && project.CreatorId == currentUserId)
        {
            // Proje sahibi ama ProjectMembers'da yok (eski projeler için)
            // Bu durumda sadece proje sahibini göster
        }

        var responses = new List<ProjectMemberResponse>();

        foreach (var member in members)
        {
            var userInfo = await userInfoService.GetUserInfoAsync(member.UserId.Value, cancellationToken);
            responses.Add(new ProjectMemberResponse
            {
                UserId = member.UserId.Value,
                UserName = userInfo?.UserName ?? string.Empty,
                Email = userInfo?.Email ?? string.Empty,
                Role = member.Role,
                JoinedDate = member.JoinedDate
            });
        }

        return responses;
    }
}

