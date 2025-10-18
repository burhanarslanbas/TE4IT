using Microsoft.EntityFrameworkCore;
using TE4IT.Application.Abstractions.Auth;
using TE4IT.Persistence.Common.Contexts;
using TE4IT.Persistence.Common.Identity;

namespace TE4IT.Infrastructure.Auth.Services;

public sealed class RefreshTokenService(AppDbContext db) : IRefreshTokenService
{
    public async Task<(string refreshToken, DateTime expiresAt)> IssueAsync(Guid userId, string createdByIp, CancellationToken ct)
    {
        var token = Convert.ToBase64String(Guid.NewGuid().ToByteArray()) + Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        var tokenHash = Convert.ToBase64String(System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(token)));
        var expires = DateTime.UtcNow.AddDays(7);
        var entity = new RefreshToken
        {
            UserId = userId,
            Token = token,
            TokenHash = tokenHash,
            ExpiresAt = expires,
            CreatedAt = DateTime.UtcNow,
            CreatedByIp = createdByIp
        };
        await db.Set<RefreshToken>().AddAsync(entity, ct);
        await db.SaveChangesAsync(ct);
        return (token, expires);
    }

    public async Task<(Guid userId, string accessToken, DateTime accessExpiresAt, string refreshToken, DateTime refreshExpiresAt)?> RefreshAsync(string token, string ipAddress, CancellationToken ct)
    {
        var now = DateTime.UtcNow;
        var tokenHash = Convert.ToBase64String(System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(token)));
        var rt = await db.Set<RefreshToken>().FirstOrDefaultAsync(x => x.TokenHash == tokenHash, ct);
        if (rt is null || rt.RevokedAt != null || rt.ExpiresAt <= now)
            return null;

        rt.RevokedAt = now;
        rt.RevokedByIp = ipAddress;
        var newToken = Convert.ToBase64String(Guid.NewGuid().ToByteArray()) + Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        var newHash = Convert.ToBase64String(System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(newToken)));
        var newExpires = now.AddDays(7);
        var replacement = new RefreshToken
        {
            UserId = rt.UserId,
            Token = newToken,
            TokenHash = newHash,
            ExpiresAt = newExpires,
            CreatedAt = now,
            CreatedByIp = ipAddress
        };
        rt.ReplacedByToken = newToken;
        await db.Set<RefreshToken>().AddAsync(replacement, ct);
        await db.SaveChangesAsync(ct);

        return (rt.UserId, string.Empty, now, newToken, newExpires);
    }

    public async Task<bool> RevokeAsync(string token, string ipAddress, string? reason, CancellationToken ct)
    {
        var tokenHash = Convert.ToBase64String(System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(token)));
        var rt = await db.Set<RefreshToken>().FirstOrDefaultAsync(x => x.TokenHash == tokenHash, ct);
        if (rt is null || rt.RevokedAt != null)
            return false;
        rt.RevokedAt = DateTime.UtcNow;
        rt.RevokedByIp = ipAddress;
        rt.RevokeReason = reason;
        await db.SaveChangesAsync(ct);
        return true;
    }
}

