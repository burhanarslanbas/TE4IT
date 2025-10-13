using MediatR;
using Microsoft.AspNetCore.Http;
using TE4IT.Application.Abstractions.Auth;

namespace TE4IT.Application.Features.Auth.Commands.Register;

public sealed class RegisterCommandHandler(
    IUserAccountService accounts,
    IUserInfoService users,
    IRolePermissionService rolePermissions,
    ITokenService tokens,
    IRefreshTokenService refreshTokens,
    IHttpContextAccessor httpContextAccessor) : IRequestHandler<RegisterCommand, RegisterCommandResponse?>
{
    public async Task<RegisterCommandResponse?> Handle(RegisterCommand request, CancellationToken ct)
    {
        var userId = await accounts.RegisterAsync(request.UserName, request.Email, request.Password, ct);

        var info = await users.GetUserInfoAsync(userId, ct);
        var roles = info?.Roles ?? Array.Empty<string>();
        var permissions = rolePermissions.GetPermissionsForRoles(roles);

        var (accessToken, expiresAt) = tokens.CreateAccessToken(
            userId,
            info?.UserName ?? request.UserName,
            info?.Email ?? request.Email,
            roles,
            permissions,
            info?.PermissionsVersion);

        // IP adresini HttpContext'ten al
        var ipAddress = httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var (refreshToken, refreshExpires) = await refreshTokens.IssueAsync(userId, ipAddress, ct);

        return new RegisterCommandResponse(
            userId,
            info?.UserName ?? request.UserName,
            info?.Email ?? request.Email,
            accessToken,
            expiresAt,
            refreshToken,
            refreshExpires
        );
    }
}


