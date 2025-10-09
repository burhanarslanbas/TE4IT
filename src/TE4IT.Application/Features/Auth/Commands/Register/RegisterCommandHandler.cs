using MediatR;
using TE4IT.Application.Abstractions.Auth;

namespace TE4IT.Application.Features.Auth.Commands.Register;

public sealed class RegisterCommandHandler(
    IUserAccountService accounts,
    IUserInfoService users,
    IRolePermissionService rolePermissions,
    ITokenService tokens,
    IRefreshTokenService refreshTokens
) : IRequestHandler<RegisterCommand, RegisterCommandResponse?>
{
    public async Task<RegisterCommandResponse?> Handle(RegisterCommand request, CancellationToken ct)
    {
        var userId = await accounts.RegisterAsync(request.Email, request.Password, ct);
        if (userId is null) return null;

        var info = await users.GetUserInfoAsync(userId.Value, ct);
        var roles = info?.Roles ?? Array.Empty<string>();
        var permissions = rolePermissions.GetPermissionsForRoles(roles);

        var (accessToken, expiresAt) = tokens.CreateAccessToken(
            userId.Value,
            info?.UserName ?? request.Email,
            info?.Email ?? request.Email,
            roles,
            permissions,
            info?.PermissionsVersion);

        var (refreshToken, refreshExpires) = await refreshTokens.IssueAsync(userId.Value, "mediator", ct);

        return new RegisterCommandResponse(
            userId.Value,
            info?.Email ?? request.Email,
            accessToken,
            expiresAt,
            refreshToken,
            refreshExpires
        );
    }
}


