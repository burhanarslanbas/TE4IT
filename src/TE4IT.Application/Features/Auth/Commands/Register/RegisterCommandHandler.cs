using MediatR;
using Microsoft.AspNetCore.Http;

using TE4IT.Application.Abstractions.Auth;
using TE4IT.Application.Abstractions.Email;
using TE4IT.Application.Abstractions.Common;

namespace TE4IT.Application.Features.Auth.Commands.Register;

public sealed class RegisterCommandHandler(
    IUserAccountService accounts,
    IUserInfoService users,
    IRolePermissionService rolePermissions,
    ITokenService tokens,
    IRefreshTokenService refreshTokens,
    IHttpContextAccessor httpContextAccessor,
    IEmailSender emailSender,
    IEmailTemplateService emailTemplate,
    IUrlService urlService) : IRequestHandler<RegisterCommand, RegisterCommandResponse?>
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

        // Hoş geldin email'i gönder (background task olarak)
        _ = Task.Run(async () =>
        {
            try
            {
                // Frontend URL'ini environment-aware olarak al
                var appUrl = urlService.GetFrontendUrl();
                var welcomeEmail = emailTemplate.GetWelcomeTemplate(
                    info?.UserName ?? request.UserName, 
                    request.Email, 
                    appUrl);
                
                await emailSender.SendAsync(request.Email, "TE4IT'e Hoş Geldiniz! 🚀", welcomeEmail, ct);
            }
            catch
            {
                // Email gönderimi başarısız olsa bile register işlemi devam etsin
            }
        }, ct);

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


