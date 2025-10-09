namespace TE4IT.Domain.Exceptions;

/// <summary>
/// Görev bağımlılık ihlali için exception
/// </summary>
public class TaskDependencyViolationException : DomainException
{
    public TaskDependencyViolationException(string message) : base(message)
    {
    }

    public TaskDependencyViolationException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
