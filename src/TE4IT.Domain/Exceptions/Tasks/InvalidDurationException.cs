using TE4IT.Domain.Exceptions.Base;

namespace TE4IT.Domain.Exceptions.Tasks;

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
