namespace TE4IT.Domain.Exceptions;

/// <summary>
/// Geçersiz süre değeri için exception
/// </summary>
public class InvalidDurationException : DomainException
{
    public InvalidDurationException(string message) : base(message)
    {
    }

    public InvalidDurationException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
