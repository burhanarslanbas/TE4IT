using TE4IT.Domain.Exceptions.Base;

namespace TE4IT.Domain.Exceptions.Auth;

/// <summary>
/// Kullanıcı giriş bilgileri geçersiz olduğunda fırlatılan exception
/// </summary>
public sealed class InvalidCredentialsException : DomainException
{
    public InvalidCredentialsException() 
        : base("E-posta adresi veya şifre hatalı.")
    {
    }

    public InvalidCredentialsException(string email) 
        : base($"E-posta adresi '{email}' için giriş bilgileri geçersiz.")
    {
        Email = email;
    }

    public string? Email { get; }
}
