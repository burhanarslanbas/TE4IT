using TE4IT.Domain.Exceptions.Base;

namespace TE4IT.Domain.Exceptions.Auth;

/// <summary>
/// Kullanıcı kayıt işlemi başarısız olduğunda fırlatılan exception
/// </summary>
public sealed class UserRegistrationFailedException : DomainException
{
    public UserRegistrationFailedException(string reason) 
        : base($"Kullanıcı kaydı başarısız: {reason}")
    {
        Reason = reason;
    }

    public UserRegistrationFailedException(string reason, Exception innerException) 
        : base($"Kullanıcı kaydı başarısız: {reason}", innerException)
    {
        Reason = reason;
    }

    public string Reason { get; }
}
