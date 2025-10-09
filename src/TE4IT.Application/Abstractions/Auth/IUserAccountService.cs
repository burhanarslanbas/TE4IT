namespace TE4IT.Application.Abstractions.Auth;

public interface IUserAccountService
{
    Task<Guid?> RegisterAsync(string email, string password, CancellationToken ct);
    Task<Guid?> ValidateCredentialsAsync(string email, string password, CancellationToken ct);
}


