// src/TE4IT.Infrastructure/Auth/Handlers/Users/GetAllUsersQueryHandler.cs
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TE4IT.Application.Common.Pagination;
using TE4IT.Application.Features.Auth.Queries.Users.GetAllUsers;
using TE4IT.Application.Features.Auth.Queries.Users.GetUserById;
using TE4IT.Persistence.Common.Identity;

namespace TE4IT.Infrastructure.Auth.Handlers.Users;

/// <summary>
/// Tüm kullanıcıları sayfalı olarak getiren handler
/// </summary>
public sealed class GetAllUsersQueryHandler(UserManager<AppUser> userManager)
    : IRequestHandler<GetAllUsersQuery, PagedResult<UserResponse>>
{
    public async Task<PagedResult<UserResponse>> Handle(
        GetAllUsersQuery request,
        CancellationToken cancellationToken)
    {
        // Toplam kullanıcı sayısını al
        var totalCount = await userManager.Users.CountAsync(cancellationToken);

        // Sayfalı kullanıcıları getir
        var users = await userManager.Users
            .OrderBy(u => u.UserName)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        // UserResponse'a dönüştür
        var userResponses = new List<UserResponse>();
        foreach (var user in users)
        {
            var roles = await userManager.GetRolesAsync(user);
            userResponses.Add(new UserResponse
            {
                Id = user.Id,
                UserName = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                EmailConfirmed = user.EmailConfirmed,
                Roles = roles
            });
        }

        return new PagedResult<UserResponse>(
            userResponses,
            totalCount,
            request.Page,
            request.PageSize
        );
    }
}