using MediatR;
using TE4IT.Abstractions.Persistence.Repositories.ProjectMembers;
using TE4IT.Application.Abstractions.Auth;
using TE4IT.Application.Abstractions.Persistence;
using TE4IT.Application.Abstractions.Persistence.Repositories.ProjectInvitations;
using TE4IT.Domain.Entities;
using TE4IT.Domain.Exceptions.Common;
using TE4IT.Domain.ValueObjects;

namespace TE4IT.Application.Features.Projects.Commands.AcceptProjectInvitation;

public sealed class AcceptProjectInvitationCommandHandler(
    IProjectInvitationReadRepository invitationReadRepository,
    IProjectInvitationWriteRepository invitationWriteRepository,
    IProjectMemberReadRepository projectMemberReadRepository,
    IProjectMemberWriteRepository projectMemberWriteRepository,
    IUnitOfWork unitOfWork,
    ICurrentUser currentUser,
    IUserInfoService userInfoService,
    IInvitationTokenService tokenService) : IRequestHandler<AcceptProjectInvitationCommand, AcceptProjectInvitationCommandResponse>
{
    public async Task<AcceptProjectInvitationCommandResponse> Handle(AcceptProjectInvitationCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = currentUser.Id ?? throw new UnauthorizedAccessException();

        // 1. Token hash'i hesapla ve daveti bul
        var tokenHash = tokenService.HashToken(request.Token);
        var invitation = await invitationReadRepository.GetByTokenHashAsync(tokenHash, cancellationToken);
        
        if (invitation == null)
            throw new BusinessRuleViolationException("Davet bulunamadı veya geçersiz.");

        // 2. Davet durumunu kontrol et
        if (invitation.IsExpired)
            throw new BusinessRuleViolationException("Bu davet süresi dolmuş. Lütfen proje sahibinden yeni bir davet isteyin.");

        if (invitation.CancelledAt != null)
            throw new BusinessRuleViolationException("Bu davet iptal edilmiş.");

        if (invitation.AcceptedAt != null)
            throw new BusinessRuleViolationException("Bu davet zaten kabul edilmiş.");

        // 3. Email eşleşmesi kontrolü
        var userInfo = await userInfoService.GetUserInfoAsync(currentUserId.Value, cancellationToken);
        if (userInfo == null)
            throw new UnauthorizedAccessException();

        var normalizedCurrentEmail = userInfo.Email.ToLowerInvariant().Trim();
        if (normalizedCurrentEmail != invitation.Email)
            throw new BusinessRuleViolationException("Bu davet sizin email adresinize gönderilmemiş. Lütfen doğru email adresi ile giriş yapın.");

        // 4. Kullanıcı zaten üye mi kontrol et
        var existingMember = await projectMemberReadRepository.GetByProjectIdAndUserIdAsync(invitation.ProjectId, userInfo.Id, cancellationToken);
        if (existingMember != null)
            throw new BusinessRuleViolationException("Zaten bu projenin üyesisiniz.");

        // 5. ProjectMember oluştur
        var projectMember = ProjectMember.Create(invitation.ProjectId, currentUserId, invitation.Role);
        await projectMemberWriteRepository.AddAsync(projectMember, cancellationToken);

        // 6. ProjectInvitation'ı güncelle (kabul edildi olarak işaretle)
        invitation.Accept(currentUserId);
        invitationWriteRepository.Update(invitation, cancellationToken);

        // 7. Değişiklikleri kaydet
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new AcceptProjectInvitationCommandResponse
        {
            ProjectMemberId = projectMember.Id,
            ProjectId = invitation.ProjectId
        };
    }
}

