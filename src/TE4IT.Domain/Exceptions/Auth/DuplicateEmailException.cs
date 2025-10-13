using TE4IT.Domain.Exceptions.Base;

namespace TE4IT.Domain.Exceptions.Auth;

/// <summary>
/// E-posta adresi zaten kullanımda olduğunda fırlatılan exception
/// </summary>
public sealed class DuplicateEmailException : DomainException
{
    public DuplicateEmailException(string email) 
        : base($"E-posta adresi '{email}' zaten kullanımda.")
    {
        Email = email;
    }

    public string Email { get; }
}
