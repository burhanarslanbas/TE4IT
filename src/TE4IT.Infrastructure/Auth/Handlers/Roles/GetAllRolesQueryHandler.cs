using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TE4IT.Application.Features.Auth.Queries.Roles.GetAllRoles;

namespace TE4IT.Infrastructure.Auth.Handlers.Roles;

/// <summary>
/// Tüm rolleri getiren handler
/// </summary>
public sealed class GetAllRolesQueryHandler(RoleManager<IdentityRole<Guid>> roleManager)
    : IRequestHandler<GetAllRolesQuery, IEnumerable<RoleResponse>>
{
    public async Task<IEnumerable<RoleResponse>> Handle(
        GetAllRolesQuery request,
        CancellationToken cancellationToken)
    {
        var roles = await roleManager.Roles
            .Select(r => new RoleResponse
            {
                Id = r.Id,
                Name = r.Name ?? string.Empty
            })
            .ToListAsync(cancellationToken);

        return roles;
    }
}

