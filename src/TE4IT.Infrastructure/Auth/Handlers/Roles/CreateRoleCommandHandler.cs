using MediatR;
using Microsoft.AspNetCore.Identity;
using TE4IT.Application.Features.Auth.Commands.Roles.CreateRole;
using TE4IT.Domain.Exceptions.Common;

namespace TE4IT.Infrastructure.Auth.Handlers.Roles;

/// <summary>
/// Yeni rol oluşturma handler'ı
/// </summary>
public sealed class CreateRoleCommandHandler(RoleManager<IdentityRole<Guid>> roleManager)
    : IRequestHandler<CreateRoleCommand, CreateRoleCommandResponse>
{
    public async Task<CreateRoleCommandResponse> Handle(
        CreateRoleCommand request,
        CancellationToken cancellationToken)
    {
        // Rol zaten var mı kontrol et
        var roleExists = await roleManager.RoleExistsAsync(request.RoleName);
        if (roleExists)
            throw new BusinessRuleViolationException($"Role '{request.RoleName}' already exists");

        // Rol oluştur
        var role = new IdentityRole<Guid>
        {
            Id = Guid.NewGuid(),
            Name = request.RoleName,
            NormalizedName = request.RoleName.ToUpperInvariant()
        };

        var result = await roleManager.CreateAsync(role);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new BusinessRuleViolationException($"Failed to create role: {errors}");
        }

        return new CreateRoleCommandResponse
        {
            RoleId = role.Id,
            RoleName = role.Name ?? string.Empty,
            Success = true
        };
    }
}

