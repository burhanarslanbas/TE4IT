// Defines the contract for issuing, refreshing, and revoking refresh tokens
namespace TE4IT.Application.Abstractions.Auth;

public interface IRefreshTokenService
{
    Task<(string refreshToken, DateTime expiresAt)> IssueAsync(Guid userId, string createdByIp, CancellationToken ct);

    Task<(Guid userId, string accessToken, DateTime accessExpiresAt, string refreshToken, DateTime refreshExpiresAt)?> RefreshAsync(string token, string ipAddress, CancellationToken ct);

    Task<bool> RevokeAsync(string token, string ipAddress, string? reason, CancellationToken ct);
}


