using MediatR;
using Microsoft.AspNetCore.Identity;
using TE4IT.Application.Features.Auth.Queries.Users.GetUserById;
using TE4IT.Persistence.Relational.Identity;

namespace TE4IT.Infrastructure.Auth.Handlers.Users;

/// <summary>
/// Kullanıcı bilgilerini getiren handler
/// </summary>
public sealed class GetUserByIdQueryHandler(UserManager<AppUser> userManager)
    : IRequestHandler<GetUserByIdQuery, UserResponse?>
{
    public async Task<UserResponse?> Handle(
        GetUserByIdQuery request,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId.ToString());
        if (user is null)
            return null;

        var roles = await userManager.GetRolesAsync(user);

        return new UserResponse
        {
            Id = user.Id,
            UserName = user.UserName ?? string.Empty,
            Email = user.Email ?? string.Empty,
            EmailConfirmed = user.EmailConfirmed,
            Roles = roles
        };
    }
}

