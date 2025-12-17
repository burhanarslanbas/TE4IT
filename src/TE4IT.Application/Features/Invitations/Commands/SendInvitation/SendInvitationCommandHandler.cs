using MediatR;
using TE4IT.Abstractions.Persistence.Repositories.ProjectMembers;
using TE4IT.Abstractions.Persistence.Repositories.Projects;
using TE4IT.Application.Abstractions.Auth;
using TE4IT.Application.Abstractions.Common;
using TE4IT.Application.Abstractions.Email;
using TE4IT.Application.Abstractions.Persistence;
using TE4IT.Application.Abstractions.Persistence.Repositories.ProjectInvitations;
using TE4IT.Domain.Entities;
using TE4IT.Domain.Enums;
using TE4IT.Domain.Exceptions.Common;
using TE4IT.Domain.Exceptions.Projects;
using TE4IT.Domain.Services;
using TE4IT.Domain.ValueObjects;

namespace TE4IT.Application.Features.Invitations.Commands.SendInvitation;

public sealed class SendInvitationCommandHandler(
    IProjectReadRepository projectRepository,
    IProjectMemberReadRepository projectMemberReadRepository,
    IProjectInvitationReadRepository invitationReadRepository,
    IProjectInvitationWriteRepository invitationWriteRepository,
    IUnitOfWork unitOfWork,
    ICurrentUser currentUser,
    IUserPermissionService userPermissionService,
    IUserAccountService userAccountService,
    IUserInfoService userInfoService,
    IEmailSender emailSender,
    IEmailTemplateService emailTemplateService,
    IUrlService urlService,
    IInvitationTokenService tokenService) : IRequestHandler<SendInvitationCommand, SendInvitationCommandResponse>
{
    private const int DefaultExpirationDays = 7;

    public async Task<SendInvitationCommandResponse> Handle(SendInvitationCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = currentUser.Id ?? throw new UnauthorizedAccessException();

        // 1. Projeyi kontrol et
        var project = await projectRepository.GetByIdAsync(request.ProjectId, cancellationToken);
        if (project == null)
            throw new ResourceNotFoundException("Project", request.ProjectId);

        // 2. Yetki kontrolü: Sadece proje sahibi veya admin davet gönderebilir
        var isAdmin = userPermissionService.IsSystemAdministrator(currentUserId);
        var userRole = userPermissionService.GetUserProjectRole(currentUserId, project);

        if (!isAdmin && (!userRole.HasValue || userRole.Value != ProjectRole.Owner))
            throw new ProjectAccessDeniedException(request.ProjectId, currentUserId.Value, "Owner permission is required to send project invitations.");

        // 3. Email'in sistemde kayıtlı olup olmadığını kontrol et
        var normalizedEmail = request.Email.ToLowerInvariant().Trim();
        var invitedUserInfo = await userAccountService.GetUserByEmailAsync(normalizedEmail, cancellationToken);
        if (invitedUserInfo == null)
            throw new BusinessRuleViolationException($"Email '{request.Email}' is not registered in the system. Please ensure the user registers first.");

        // 4. Kullanıcı zaten üye mi kontrol et
        var existingMember = await projectMemberReadRepository.GetByProjectIdAndUserIdAsync(request.ProjectId, invitedUserInfo.Id, cancellationToken);
        if (existingMember != null)
            throw new BusinessRuleViolationException("User is already a member of this project.");

        // 5. Bekleyen davet var mı kontrol et
        var existingInvitation = await invitationReadRepository.GetByProjectIdAndEmailAsync(request.ProjectId, normalizedEmail, cancellationToken);
        if (existingInvitation != null && existingInvitation.IsPending)
            throw new BusinessRuleViolationException("A pending invitation has already been sent to this email address.");

        // 6. Token oluştur
        var token = tokenService.GenerateToken();
        var tokenHash = tokenService.HashToken(token);

        // 7. ProjectInvitation entity'si oluştur
        var invitation = ProjectInvitation.Create(
            request.ProjectId,
            normalizedEmail,
            request.Role,
            currentUserId,
            token,
            tokenHash,
            DefaultExpirationDays);

        // 8. Database'e kaydet
        await invitationWriteRepository.AddAsync(invitation, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        // 9. Email gönder
        try
        {
            var frontendUrl = urlService.GetFrontendUrl();
            var acceptLink = $"{frontendUrl}/accept-invitation?token={Uri.EscapeDataString(token)}";
            var inviterInfo = await userInfoService.GetUserInfoAsync(currentUserId.Value, cancellationToken);
            var inviterName = inviterInfo?.UserName ?? inviterInfo?.Email ?? "A user";

            var emailBody = emailTemplateService.GetProjectInvitationTemplate(
                project.Title,
                inviterName,
                request.Role,
                acceptLink,
                invitation.ExpiresAt);

            await emailSender.SendAsync(
                request.Email,
                $"Project Invitation: {project.Title}",
                emailBody,
                cancellationToken);
        }
        catch (Exception)
        {
            // Email gönderimi başarısız olsa bile davet kaydedilmiştir
            // Exception fırlatılmaz (invitation zaten kaydedildi)
            // Note: Logging should be handled at Infrastructure layer if needed
        }

        return new SendInvitationCommandResponse
        {
            InvitationId = invitation.Id,
            Email = invitation.Email,
            ExpiresAt = invitation.ExpiresAt
        };
    }
}
