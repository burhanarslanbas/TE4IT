using MediatR;
using Microsoft.AspNetCore.Identity;
using TE4IT.Application.Features.Auth.Commands.Users.RemoveRoleFromUser;
using TE4IT.Domain.Exceptions.Common;
using TE4IT.Persistence.Relational.Identity;

namespace TE4IT.Infrastructure.Auth.Handlers.Users;

/// <summary>
/// Kullanıcıdan rol kaldırma handler'ı
/// </summary>
public sealed class RemoveRoleFromUserCommandHandler(UserManager<AppUser> userManager)
    : IRequestHandler<RemoveRoleFromUserCommand>
{
    public async Task Handle(
        RemoveRoleFromUserCommand request,
        CancellationToken cancellationToken)
    {
        // User var mı kontrol et
        var user = await userManager.FindByIdAsync(request.UserId.ToString());
        if (user is null)
            throw new ResourceNotFoundException($"User with ID {request.UserId} not found");

        // Kullanıcının bu rolü var mı?
        var isInRole = await userManager.IsInRoleAsync(user, request.RoleName);
        if (!isInRole)
            throw new BusinessRuleViolationException($"User does not have the role '{request.RoleName}'");

        // Rolü kaldır
        var result = await userManager.RemoveFromRoleAsync(user, request.RoleName);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new BusinessRuleViolationException($"Failed to remove role: {errors}");
        }

        // SecurityStamp güncelle
        await userManager.UpdateSecurityStampAsync(user);
    }
}

