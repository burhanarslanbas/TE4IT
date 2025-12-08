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

namespace TE4IT.Application.Features.Projects.Commands.InviteProjectMember;

public sealed class InviteProjectMemberCommandHandler(
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
    IInvitationTokenService tokenService) : IRequestHandler<InviteProjectMemberCommand, InviteProjectMemberCommandResponse>
{
    private const int DefaultExpirationDays = 7;

    public async Task<InviteProjectMemberCommandResponse> Handle(InviteProjectMemberCommand request, CancellationToken cancellationToken)
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
            throw new ProjectAccessDeniedException(request.ProjectId, currentUserId.Value, "Proje üyesi davet göndermek için Owner yetkisi gereklidir.");

        // 3. Email'in sistemde kayıtlı olup olmadığını kontrol et
        var normalizedEmail = request.Email.ToLowerInvariant().Trim();
        var invitedUserInfo = await userAccountService.GetUserByEmailAsync(normalizedEmail, cancellationToken);
        if (invitedUserInfo == null)
            throw new BusinessRuleViolationException($"Email '{request.Email}' sistemde kayıtlı değil. Lütfen önce kullanıcının kayıt olmasını sağlayın.");

        // 4. Kullanıcı zaten üye mi kontrol et
        var existingMember = await projectMemberReadRepository.GetByProjectIdAndUserIdAsync(request.ProjectId, invitedUserInfo.Id, cancellationToken);
        var invitedUserId = (UserId)invitedUserInfo.Id;
        if (existingMember != null)
            throw new BusinessRuleViolationException("Bu kullanıcı zaten projenin üyesidir.");

        // 5. Bekleyen davet var mı kontrol et
        var existingInvitation = await invitationReadRepository.GetByProjectIdAndEmailAsync(request.ProjectId, normalizedEmail, cancellationToken);
        if (existingInvitation != null && existingInvitation.IsPending)
            throw new BusinessRuleViolationException("Bu email adresine zaten bekleyen bir davet gönderilmiş.");

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
            var inviterName = inviterInfo?.UserName ?? inviterInfo?.Email ?? "Bir kullanıcı";

            var emailBody = emailTemplateService.GetProjectInvitationTemplate(
                project.Title,
                inviterName,
                request.Role,
                acceptLink,
                invitation.ExpiresAt);

            await emailSender.SendAsync(
                request.Email,
                $"Proje Daveti: {project.Title}",
                emailBody,
                cancellationToken);
        }
        catch (Exception)
        {
            // Email gönderimi başarısız olsa bile davet kaydedilmiştir
            // Burada loglama yapılabilir ancak exception fırlatmayız
        }

        return new InviteProjectMemberCommandResponse
        {
            InvitationId = invitation.Id,
            Email = invitation.Email,
            ExpiresAt = invitation.ExpiresAt
        };
    }
}

