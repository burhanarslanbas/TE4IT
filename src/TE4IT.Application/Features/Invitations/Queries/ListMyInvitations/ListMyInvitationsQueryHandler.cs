using MediatR;
using TE4IT.Abstractions.Persistence.Repositories.Projects;
using TE4IT.Application.Abstractions.Auth;
using TE4IT.Application.Abstractions.Persistence.Repositories.ProjectInvitations;
using TE4IT.Domain.Exceptions.Common;

namespace TE4IT.Application.Features.Invitations.Queries.ListMyInvitations;

public sealed class ListMyInvitationsQueryHandler(
    IProjectInvitationReadRepository invitationReadRepository,
    IProjectReadRepository projectRepository,
    ICurrentUser currentUser,
    IUserInfoService userInfoService) : IRequestHandler<ListMyInvitationsQuery, List<MyInvitationResponse>>
{
    public async Task<List<MyInvitationResponse>> Handle(ListMyInvitationsQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = currentUser.Id ?? throw new UnauthorizedAccessException();

        // 1. Kullanıcı bilgilerini getir
        var userInfo = await userInfoService.GetUserInfoAsync(currentUserId.Value, cancellationToken);
        if (userInfo == null)
            throw new UnauthorizedAccessException();

        var userEmail = userInfo.Email.ToLowerInvariant().Trim();

        // 2. Kullanıcının tüm davetlerini getir
        var allInvitations = await invitationReadRepository.GetByEmailAsync(userEmail, cancellationToken);

        // 3. Status filtresi uygula
        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            var statusFilter = request.Status.ToLowerInvariant();
            allInvitations = allInvitations.Where(i => i.Status.ToLowerInvariant() == statusFilter).ToList();
        }

        // 4. Response listesi oluştur
        var responses = new List<MyInvitationResponse>();

        foreach (var invitation in allInvitations)
        {
            // Proje bilgilerini getir
            var project = await projectRepository.GetByIdAsync(invitation.ProjectId, cancellationToken);
            if (project == null)
                continue; // Proje bulunamazsa atla

            // Davet eden kullanıcı bilgilerini getir
            var inviterInfo = await userInfoService.GetUserInfoAsync(invitation.InvitedByUserId.Value, cancellationToken);
            var inviterName = inviterInfo?.UserName ?? inviterInfo?.Email;

            responses.Add(new MyInvitationResponse(
                invitation.Id,
                invitation.ProjectId,
                project.Title,
                invitation.Role,
                invitation.ExpiresAt,
                inviterName,
                invitation.CreatedDate,
                invitation.Status,
                invitation.AcceptedAt,
                invitation.CancelledAt));
        }

        return responses;
    }
}
