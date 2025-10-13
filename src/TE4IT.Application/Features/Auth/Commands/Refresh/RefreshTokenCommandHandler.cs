using MediatR;
using Microsoft.AspNetCore.Http;
using TE4IT.Application.Abstractions.Auth;

namespace TE4IT.Application.Features.Auth.Commands.Refresh;

public sealed class RefreshTokenCommandHandler(
    IRefreshTokenService refreshTokens, 
    ITokenService tokenService, 
    IUserInfoService userInfo, 
    IRolePermissionService rolePermissions,
    IHttpContextAccessor httpContextAccessor) : IRequestHandler<RefreshTokenCommand, RefreshTokenCommandResponse>
{
    public async Task<RefreshTokenCommandResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        // IP adresini HttpContext'ten al
        var ip = httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var result = await refreshTokens.RefreshAsync(request.RefreshToken, ip, cancellationToken);
        if (result is null) throw new UnauthorizedAccessException();

        var (userId, _, _, newRefresh, refreshExpires) = result.Value;
        var info = await userInfo.GetUserInfoAsync(userId, cancellationToken) ?? throw new UnauthorizedAccessException();
        var permissions = rolePermissions.GetPermissionsForRoles(info.Roles);
        var (accessToken, accessExpiresAt) = tokenService.CreateAccessToken(info.Id, info.UserName, info.Email, info.Roles, permissions, info.PermissionsVersion);
        return new RefreshTokenCommandResponse(accessToken, accessExpiresAt, newRefresh, refreshExpires);
    }
}


