using MediatR;
using TE4IT.Abstractions.Persistence.Repositories.ProjectMembers;
using TE4IT.Application.Abstractions.Auth;
using TE4IT.Application.Abstractions.Persistence;
using TE4IT.Application.Abstractions.Persistence.Repositories.ProjectInvitations;
using TE4IT.Domain.Entities;
using TE4IT.Domain.Exceptions.Common;
using TE4IT.Domain.ValueObjects;

namespace TE4IT.Application.Features.Invitations.Commands.AcceptInvitation;

public sealed class AcceptInvitationCommandHandler(
    IProjectInvitationReadRepository invitationReadRepository,
    IProjectInvitationWriteRepository invitationWriteRepository,
    IProjectMemberReadRepository projectMemberReadRepository,
    IProjectMemberWriteRepository projectMemberWriteRepository,
    IUnitOfWork unitOfWork,
    ICurrentUser currentUser,
    IUserInfoService userInfoService,
    IInvitationTokenService tokenService) : IRequestHandler<AcceptInvitationCommand, AcceptInvitationCommandResponse>
{
    public async Task<AcceptInvitationCommandResponse> Handle(AcceptInvitationCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = currentUser.Id ?? throw new UnauthorizedAccessException();

        // 1. Token hash'i hesapla ve daveti bul
        var tokenHash = tokenService.HashToken(request.Token);
        var invitation = await invitationReadRepository.GetByTokenHashAsync(tokenHash, cancellationToken);
        
        if (invitation == null)
            throw new ResourceNotFoundException("Project invitation not found or invalid token.");

        // 2. Davet durumunu kontrol et
        if (invitation.IsExpired)
            throw new BusinessRuleViolationException("Invitation has expired. Please request a new invitation from the project owner.");

        if (invitation.CancelledAt != null)
            throw new BusinessRuleViolationException("Invitation has been cancelled.");

        if (invitation.AcceptedAt != null)
            throw new BusinessRuleViolationException("Invitation has already been accepted.");

        // 3. Email eşleşmesi kontrolü
        var userInfo = await userInfoService.GetUserInfoAsync(currentUserId.Value, cancellationToken);
        if (userInfo == null)
            throw new UnauthorizedAccessException();

        var normalizedCurrentEmail = userInfo.Email.ToLowerInvariant().Trim();
        if (normalizedCurrentEmail != invitation.Email)
            throw new BusinessRuleViolationException("This invitation was not sent to your email address. Please sign in with the correct email address.");

        // 4. Kullanıcı zaten üye mi kontrol et
        var existingMember = await projectMemberReadRepository.GetByProjectIdAndUserIdAsync(invitation.ProjectId, userInfo.Id, cancellationToken);
        if (existingMember != null)
            throw new BusinessRuleViolationException("You are already a member of this project.");

        // 5. ProjectMember oluştur
        var projectMember = ProjectMember.Create(invitation.ProjectId, currentUserId, invitation.Role);
        await projectMemberWriteRepository.AddAsync(projectMember, cancellationToken);

        // 6. ProjectInvitation'ı güncelle (kabul edildi olarak işaretle)
        invitation.Accept(currentUserId);
        invitationWriteRepository.Update(invitation, cancellationToken);

        // 7. Değişiklikleri kaydet
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new AcceptInvitationCommandResponse
        {
            ProjectMemberId = projectMember.Id,
            ProjectId = invitation.ProjectId
        };
    }
}
