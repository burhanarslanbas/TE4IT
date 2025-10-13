using TE4IT.Domain.Exceptions.Base;

namespace TE4IT.Domain.Exceptions.Auth;

/// <summary>
/// Kullanıcı adı zaten kullanımda olduğunda fırlatılan exception
/// </summary>
public sealed class DuplicateUserNameException : DomainException
{
    public DuplicateUserNameException(string userName) 
        : base($"Kullanıcı adı '{userName}' zaten kullanımda.")
    {
        UserName = userName;
    }

    public string UserName { get; }
}
