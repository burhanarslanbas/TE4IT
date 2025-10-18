using MediatR;
using Microsoft.AspNetCore.Http;
using TE4IT.Application.Abstractions.Auth;

namespace TE4IT.Application.Features.Auth.Commands.RevokeRefreshToken;

/// <summary>
/// Refresh token iptal etme komut handler'ı
/// </summary>
public sealed class RevokeRefreshTokenCommandHandler(
    IRefreshTokenService refreshTokens,
    IHttpContextAccessor httpContextAccessor) : IRequestHandler<RevokeRefreshTokenCommand, bool>
{
    public async Task<bool> Handle(RevokeRefreshTokenCommand request, CancellationToken cancellationToken)
    {
        // IP adresini HttpContext'ten al
        var ipAddress = httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        
        // Reason'ı belirle
        var reason = "user requested";
        
        // Refresh token'ı iptal et
        return await refreshTokens.RevokeAsync(request.RefreshToken, ipAddress, reason, cancellationToken);
    }
}


