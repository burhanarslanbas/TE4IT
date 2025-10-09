using Microsoft.AspNetCore.Identity;
using TE4IT.Application.Abstractions.Auth;
using TE4IT.Persistence.Relational.Identity;
using AppUserInfo = TE4IT.Application.Abstractions.Auth.UserInfo;

namespace TE4IT.Infrastructure.Auth.Services;

public sealed class UserInfoService(UserManager<AppUser> userManager) : IUserInfoService
{
    public async Task<AppUserInfo?> GetUserInfoAsync(Guid userId, CancellationToken ct)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null) return null;
        var roles = await userManager.GetRolesAsync(user);
        // PermissionsVersion: örnek olarak SecurityStamp kullanıyoruz; daha gelişmiş senaryoda ayrı bir alan saklanabilir
        var permissionsVersion = await userManager.GetSecurityStampAsync(user);
        return new AppUserInfo(user.Id, user.UserName ?? user.Email ?? string.Empty, user.Email ?? string.Empty, roles, permissionsVersion);
    }
}

