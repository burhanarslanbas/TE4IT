namespace TE4IT.Application.Abstractions.Auth;

/// <summary>
/// Proje davetleri için token oluşturma ve hash'leme servisi
/// </summary>
public interface IInvitationTokenService
{
    /// <summary>
    /// Güvenli davet token'ı oluşturur
    /// </summary>
    string GenerateToken();

    /// <summary>
    /// Token'ı SHA256 ile hash'ler
    /// </summary>
    string HashToken(string token);
}

