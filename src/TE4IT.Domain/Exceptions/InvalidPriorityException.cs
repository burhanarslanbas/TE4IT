namespace TE4IT.Domain.Exceptions;

/// <summary>
/// Geçersiz öncelik değeri için exception
/// </summary>
public class InvalidPriorityException : DomainException
{
    public InvalidPriorityException(string message) : base(message)
    {
    }

    public InvalidPriorityException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
