namespace TE4IT.Domain.Exceptions.Base;

/// <summary>
/// Domain katmanı için temel exception sınıfı
/// </summary>
public abstract class DomainException : Exception
{
    protected DomainException(string message) : base(message)
    {
    }

    protected DomainException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
