using MediatR;
using Microsoft.AspNetCore.Identity;
using TE4IT.Application.Features.Auth.Commands.Roles.UpdateRole;
using TE4IT.Domain.Exceptions.Common;

namespace TE4IT.Infrastructure.Auth.Handlers.Roles;

/// <summary>
/// Rol güncelleme handler'ı
/// </summary>
public sealed class UpdateRoleCommandHandler(RoleManager<IdentityRole<Guid>> roleManager)
    : IRequestHandler<UpdateRoleCommand, UpdateRoleCommandResponse>
{
    public async Task<UpdateRoleCommandResponse> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
    {
        var role = await roleManager.FindByIdAsync(request.RoleId.ToString());
        if (role is null)
            throw new ResourceNotFoundException($"Role with ID {request.RoleId} not found");
        role.Name = request.RoleName;
        var result = await roleManager.UpdateAsync(role);
        if (!result.Succeeded)
            throw new BusinessRuleViolationException($"Failed to update role: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        return new UpdateRoleCommandResponse
        {
            RoleId = role.Id,
            RoleName = role.Name ?? string.Empty,
            Success = true
        };
    }
}