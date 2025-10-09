namespace TE4IT.Domain.Exceptions;

/// <summary>
/// Geçersiz email adresi için exception
/// </summary>
public class InvalidEmailException : DomainException
{
    public InvalidEmailException(string message) : base(message)
    {
    }

    public InvalidEmailException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
