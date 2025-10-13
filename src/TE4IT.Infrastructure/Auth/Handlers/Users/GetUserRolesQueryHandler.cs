using MediatR;
using Microsoft.AspNetCore.Identity;
using TE4IT.Application.Features.Auth.Queries.Users.GetUserRoles;
using TE4IT.Domain.Exceptions.Common;
using TE4IT.Persistence.Relational.Identity;

namespace TE4IT.Infrastructure.Auth.Handlers.Users;

/// <summary>
/// Kullanıcının rollerini getiren handler
/// </summary>
public sealed class GetUserRolesQueryHandler(UserManager<AppUser> userManager)
    : IRequestHandler<GetUserRolesQuery, IEnumerable<string>>
{
    public async Task<IEnumerable<string>> Handle(
        GetUserRolesQuery request,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId.ToString());
        if (user is null)
            throw new ResourceNotFoundException($"User with ID {request.UserId} not found");

        var roles = await userManager.GetRolesAsync(user);
        return roles;
    }
}

