using System.Security.Cryptography;
using System.Text;
using TE4IT.Application.Abstractions.Auth;

namespace TE4IT.Infrastructure.Auth.Services;

/// <summary>
/// Proje davetleri için token oluşturma ve hash'leme servisi
/// </summary>
public sealed class InvitationTokenService : IInvitationTokenService
{
    /// <summary>
    /// Güvenli davet token'ı oluşturur
    /// </summary>
    public string GenerateToken()
    {
        // İki GUID birleştir ve Base64'e çevir
        var bytes1 = Guid.NewGuid().ToByteArray();
        var bytes2 = Guid.NewGuid().ToByteArray();
        var combined = bytes1.Concat(bytes2).ToArray();
        return Convert.ToBase64String(combined);
    }

    /// <summary>
    /// Token'ı SHA256 ile hash'ler
    /// </summary>
    public string HashToken(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            throw new ArgumentException("Token cannot be empty.", nameof(token));

        var bytes = Encoding.UTF8.GetBytes(token);
        var hash = SHA256.HashData(bytes);
        return Convert.ToBase64String(hash);
    }
}

