using MediatR;
using Microsoft.AspNetCore.Identity;
using TE4IT.Application.Features.Auth.Commands.Roles.DeleteRole;
using TE4IT.Domain.Exceptions;

namespace TE4IT.Infrastructure.Auth.Handlers.Roles;

/// <summary>
/// Rol silme handler'ı
/// </summary>
public sealed class DeleteRoleCommandHandler(RoleManager<IdentityRole<Guid>> roleManager)
    : IRequestHandler<DeleteRoleCommand>
{
    public async Task Handle(
        DeleteRoleCommand request,
        CancellationToken cancellationToken)
    {
        var role = await roleManager.FindByIdAsync(request.RoleId.ToString());
        if (role is null)
            throw new ResourceNotFoundException($"Role with ID {request.RoleId} not found");

        var result = await roleManager.DeleteAsync(role);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new BusinessRuleViolationException($"Failed to delete role: {errors}");
        }
    }
}

