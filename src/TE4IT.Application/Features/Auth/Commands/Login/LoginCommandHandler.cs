using MediatR;
using TE4IT.Application.Abstractions.Auth;

namespace TE4IT.Application.Features.Auth.Commands.Login;

public sealed class LoginCommandHandler(
    IUserAccountService accounts,
    IUserInfoService users,
    IRolePermissionService rolePermissions,
    ITokenService tokens,
    IRefreshTokenService refreshTokens
) : IRequestHandler<LoginCommand, LoginCommandResponse?>
{
    public async Task<LoginCommandResponse?> Handle(LoginCommand request, CancellationToken ct)
    {
        var userId = await accounts.ValidateCredentialsAsync(request.Email, request.Password, ct);
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

        return new LoginCommandResponse(
            userId.Value,
            info?.Email ?? request.Email,
            accessToken,
            expiresAt,
            refreshToken,
            refreshExpires
        );
    }
}


