using MediatR;
using Microsoft.AspNetCore.Identity;
using TE4IT.Application.Features.Auth.Queries.Roles.GetAllRoles;
using TE4IT.Application.Features.Auth.Queries.Roles.GetRoleById;

namespace TE4IT.Infrastructure.Auth.Handlers.Roles;

/// <summary>
/// Rol bilgilerini getiren handler
/// </summary>
public sealed class GetRoleByIdQueryHandler(RoleManager<IdentityRole<Guid>> roleManager)
    : IRequestHandler<GetRoleByIdQuery, RoleResponse?>
{
    public async Task<RoleResponse?> Handle(
        GetRoleByIdQuery request,
        CancellationToken cancellationToken)
    {
        var role = await roleManager.FindByIdAsync(request.RoleId.ToString());
        if (role is null)
            return null;

        return new RoleResponse
        {
            Id = role.Id,
            Name = role.Name ?? string.Empty
        };
    }
}

