using MediatR;
using Microsoft.AspNetCore.Identity;
using TE4IT.Application.Features.Auth.Commands.Users.AssignRoleToUser;
using TE4IT.Domain.Exceptions;
using TE4IT.Persistence.Relational.Identity;

namespace TE4IT.Infrastructure.Auth.Handlers.Users;

/// <summary>
/// Kullanıcıya rol atama handler'ı
/// </summary>
public sealed class AssignRoleToUserCommandHandler(
    UserManager<AppUser> userManager,
    RoleManager<IdentityRole<Guid>> roleManager)
    : IRequestHandler<AssignRoleToUserCommand, AssignRoleToUserCommandResponse>
{
    public async Task<AssignRoleToUserCommandResponse> Handle(
        AssignRoleToUserCommand request,
        CancellationToken cancellationToken)
    {
        // User var mı kontrol et
        var user = await userManager.FindByIdAsync(request.UserId.ToString());
        if (user is null)
            throw new ResourceNotFoundException($"User with ID {request.UserId} not found");

        // Role var mı kontrol et
        var roleExists = await roleManager.RoleExistsAsync(request.RoleName);
        if (!roleExists)
            throw new ResourceNotFoundException($"Role '{request.RoleName}' not found");

        // Kullanıcı zaten bu role'e sahip mi?
        var isInRole = await userManager.IsInRoleAsync(user, request.RoleName);
        if (isInRole)
            throw new BusinessRuleViolationException($"User already has the role '{request.RoleName}'");

        // Rol ata
        var result = await userManager.AddToRoleAsync(user, request.RoleName);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new BusinessRuleViolationException($"Failed to assign role: {errors}");
        }

        // SecurityStamp güncelle (eski token'lar invalidate olsun)
        await userManager.UpdateSecurityStampAsync(user);

        return new AssignRoleToUserCommandResponse
        {
            UserId = request.UserId,
            RoleName = request.RoleName,
            Success = true,
            Message = $"Role '{request.RoleName}' assigned to user successfully"
        };
    }
}

