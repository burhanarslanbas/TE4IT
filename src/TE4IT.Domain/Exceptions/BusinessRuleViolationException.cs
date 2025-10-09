namespace TE4IT.Domain.Exceptions;

/// <summary>
/// İş kuralı ihlali durumlarında fırlatılan exception
/// </summary>
public sealed class BusinessRuleViolationException : DomainException
{
    public BusinessRuleViolationException(string message) : base(message)
    {
    }

    public BusinessRuleViolationException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

