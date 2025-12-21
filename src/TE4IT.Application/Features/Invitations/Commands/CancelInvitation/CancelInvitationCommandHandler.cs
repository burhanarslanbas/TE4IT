using MediatR;
using TE4IT.Abstractions.Persistence.Repositories.Projects;
using TE4IT.Application.Abstractions.Auth;
using TE4IT.Application.Abstractions.Persistence;
using TE4IT.Application.Abstractions.Persistence.Repositories.ProjectInvitations;
using TE4IT.Domain.Enums;
using TE4IT.Domain.Exceptions.Common;
using TE4IT.Domain.Exceptions.Projects;
using TE4IT.Domain.Services;

namespace TE4IT.Application.Features.Invitations.Commands.CancelInvitation;

public sealed class CancelInvitationCommandHandler(
    IProjectReadRepository projectRepository,
    IProjectInvitationReadRepository invitationReadRepository,
    IProjectInvitationWriteRepository invitationWriteRepository,
    IUnitOfWork unitOfWork,
    ICurrentUser currentUser,
    IUserPermissionService userPermissionService) : IRequestHandler<CancelInvitationCommand, bool>
{
    public async Task<bool> Handle(CancelInvitationCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = currentUser.Id ?? throw new UnauthorizedAccessException();

        // 1. Projeyi kontrol et
        var project = await projectRepository.GetByIdAsync(request.ProjectId, cancellationToken);
        if (project == null)
            throw new ResourceNotFoundException("Project", request.ProjectId);

        // 2. Yetki kontrolü: Sadece proje sahibi veya admin iptal edebilir
        var isAdmin = userPermissionService.IsSystemAdministrator(currentUserId);
        var userRole = userPermissionService.GetUserProjectRole(currentUserId, project);

        if (!isAdmin && (!userRole.HasValue || userRole.Value != ProjectRole.Owner))
            throw new ProjectAccessDeniedException(request.ProjectId, currentUserId.Value, "Owner permission is required to cancel invitations.");

        // 3. Daveti bul
        var invitation = await invitationReadRepository.GetByIdAsync(request.InvitationId, cancellationToken);
        if (invitation == null)
            return false;

        // 4. Davetin bu projeye ait olduğunu kontrol et
        if (invitation.ProjectId != request.ProjectId)
            throw new BusinessRuleViolationException("This invitation does not belong to this project.");

        // 5. Kabul edilmiş davet iptal edilemez
        if (invitation.AcceptedAt != null)
            throw new BusinessRuleViolationException("Accepted invitations cannot be cancelled.");

        // 6. Zaten iptal edilmiş mi kontrol et
        if (invitation.CancelledAt != null)
            throw new BusinessRuleViolationException("This invitation has already been cancelled.");

        // 7. Daveti iptal et
        invitation.Cancel();
        invitationWriteRepository.Update(invitation, cancellationToken);

        // 8. Değişiklikleri kaydet
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
