namespace TE4IT.Application.Abstractions.Auth;

public interface IUserAccountService
{
    Task<Guid> RegisterAsync(string userName, string email, string password, CancellationToken ct);
    Task<Guid> ValidateCredentialsAsync(string email, string password, CancellationToken ct);

    // Parola sıfırlama akışı
    Task<string?> GeneratePasswordResetTokenAsync(string email, CancellationToken ct);
    Task<bool> ResetPasswordAsync(string email, string token, string newPassword, CancellationToken ct);
}


