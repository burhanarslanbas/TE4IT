using TE4IT.Domain.Exceptions.Base;

namespace TE4IT.Domain.Exceptions.Tasks;

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
