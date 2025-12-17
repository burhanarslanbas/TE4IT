using MediatR;
using TE4IT.Abstractions.Persistence.Repositories.Projects;
using TE4IT.Application.Abstractions.Auth;
using TE4IT.Application.Abstractions.Persistence.Repositories.ProjectInvitations;
using TE4IT.Domain.Exceptions.Common;

namespace TE4IT.Application.Features.Invitations.Queries.ListMyPendingInvitations;

public sealed class ListMyPendingInvitationsQueryHandler(
    IProjectInvitationReadRepository invitationReadRepository,
    IProjectReadRepository projectRepository,
    ICurrentUser currentUser,
    IUserInfoService userInfoService) : IRequestHandler<ListMyPendingInvitationsQuery, List<MyPendingInvitationResponse>>
{
    public async Task<List<MyPendingInvitationResponse>> Handle(ListMyPendingInvitationsQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = currentUser.Id ?? throw new UnauthorizedAccessException();

        // 1. Kullanıcı bilgilerini getir
        var userInfo = await userInfoService.GetUserInfoAsync(currentUserId.Value, cancellationToken);
        if (userInfo == null)
            throw new UnauthorizedAccessException();

        var userEmail = userInfo.Email.ToLowerInvariant().Trim();

        // 2. Kullanıcının tüm davetlerini getir
        var allInvitations = await invitationReadRepository.GetByEmailAsync(userEmail, cancellationToken);

        // 3. Sadece pending olanları filtrele
        var pendingInvitations = allInvitations.Where(i => i.IsPending).ToList();

        // 4. Response listesi oluştur
        var responses = new List<MyPendingInvitationResponse>();

        foreach (var invitation in pendingInvitations)
        {
            // Proje bilgilerini getir
            var project = await projectRepository.GetByIdAsync(invitation.ProjectId, cancellationToken);
            if (project == null)
                continue; // Proje bulunamazsa atla

            // Davet eden kullanıcı bilgilerini getir
            var inviterInfo = await userInfoService.GetUserInfoAsync(invitation.InvitedByUserId.Value, cancellationToken);
            var inviterName = inviterInfo?.UserName ?? inviterInfo?.Email ?? "A user";

            responses.Add(new MyPendingInvitationResponse(
                invitation.Id,
                invitation.ProjectId,
                project.Title,
                invitation.Role,
                invitation.ExpiresAt,
                inviterName,
                invitation.CreatedDate));
        }

        return responses;
    }
}
