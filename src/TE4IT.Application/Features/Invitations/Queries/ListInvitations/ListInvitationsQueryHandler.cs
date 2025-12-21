using MediatR;
using TE4IT.Abstractions.Persistence.Repositories.Projects;
using TE4IT.Application.Abstractions.Auth;
using TE4IT.Application.Abstractions.Persistence.Repositories.ProjectInvitations;
using TE4IT.Domain.Enums;
using TE4IT.Domain.Exceptions.Common;
using TE4IT.Domain.Exceptions.Projects;
using TE4IT.Domain.Services;

namespace TE4IT.Application.Features.Invitations.Queries.ListInvitations;

public sealed class ListInvitationsQueryHandler(
    IProjectReadRepository projectRepository,
    IProjectInvitationReadRepository invitationReadRepository,
    IUserInfoService userInfoService,
    ICurrentUser currentUser,
    IUserPermissionService userPermissionService) : IRequestHandler<ListInvitationsQuery, List<InvitationListItemResponse>>
{
    public async Task<List<InvitationListItemResponse>> Handle(ListInvitationsQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = currentUser.Id ?? throw new UnauthorizedAccessException();

        // 1. Projeyi kontrol et
        var project = await projectRepository.GetByIdAsync(request.ProjectId, cancellationToken);
        if (project == null)
            throw new ResourceNotFoundException("Project", request.ProjectId);

        // 2. Yetki kontrolü: Sadece proje sahibi veya admin listeleyebilir
        var isAdmin = userPermissionService.IsSystemAdministrator(currentUserId);
        var userRole = userPermissionService.GetUserProjectRole(currentUserId, project);

        if (!isAdmin && (!userRole.HasValue || userRole.Value != ProjectRole.Owner))
            throw new ProjectAccessDeniedException(request.ProjectId, currentUserId.Value, "Owner permission is required to list invitations.");

        // 3. Davetleri getir
        var invitations = await invitationReadRepository.GetByProjectIdAsync(request.ProjectId, cancellationToken);

        // 4. Status filtresi uygula
        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            var statusFilter = request.Status.ToLowerInvariant();
            invitations = invitations.Where(i => i.Status.ToLowerInvariant() == statusFilter).ToList();
        }

        // 5. Response listesi oluştur
        var responses = new List<InvitationListItemResponse>();

        foreach (var invitation in invitations)
        {
            var inviterInfo = await userInfoService.GetUserInfoAsync(invitation.InvitedByUserId.Value, cancellationToken);
            var inviterName = inviterInfo?.UserName ?? inviterInfo?.Email ?? "A user";

            responses.Add(new InvitationListItemResponse
            {
                InvitationId = invitation.Id,
                Email = invitation.Email,
                Role = invitation.Role,
                Status = invitation.Status,
                CreatedDate = invitation.CreatedDate,
                ExpiresAt = invitation.ExpiresAt,
                AcceptedAt = invitation.AcceptedAt,
                InvitedByUserName = inviterName
            });
        }

        return responses;
    }
}
