using MediatR;
using TE4IT.Abstractions.Persistence.Repositories.Projects;
using TE4IT.Application.Abstractions.Auth;
using TE4IT.Application.Abstractions.Persistence.Repositories.ProjectInvitations;

namespace TE4IT.Application.Features.Projects.Queries.GetProjectInvitationByToken;

public sealed class GetProjectInvitationByTokenQueryHandler(
    IProjectInvitationReadRepository invitationReadRepository,
    IProjectReadRepository projectRepository,
    IUserInfoService userInfoService,
    IInvitationTokenService tokenService) : IRequestHandler<GetProjectInvitationByTokenQuery, ProjectInvitationResponse?>
{
    public async Task<ProjectInvitationResponse?> Handle(GetProjectInvitationByTokenQuery request, CancellationToken cancellationToken)
    {
        // 1. Token hash'i hesapla ve daveti bul
        var tokenHash = tokenService.HashToken(request.Token);
        var invitation = await invitationReadRepository.GetByTokenHashAsync(tokenHash, cancellationToken);

        if (invitation == null)
            return null;

        // 2. Davet geçerli mi kontrol et (expired veya cancelled)
        if (invitation.IsExpired || invitation.CancelledAt != null)
            return null;

        // 3. Proje bilgilerini getir
        var project = await projectRepository.GetByIdAsync(invitation.ProjectId, cancellationToken);
        if (project == null)
            return null;

        // 4. Davet eden kullanıcı bilgilerini getir
        var inviterInfo = await userInfoService.GetUserInfoAsync(invitation.InvitedByUserId.Value, cancellationToken);
        var inviterName = inviterInfo?.UserName ?? inviterInfo?.Email ?? "Bir kullanıcı";

        // 5. Response oluştur
        return new ProjectInvitationResponse
        {
            InvitationId = invitation.Id,
            ProjectId = invitation.ProjectId,
            ProjectTitle = project.Title,
            Email = invitation.Email,
            Role = invitation.Role,
            ExpiresAt = invitation.ExpiresAt,
            InvitedByUserName = inviterName
        };
    }
}

