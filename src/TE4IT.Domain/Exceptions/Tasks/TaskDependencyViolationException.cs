using TE4IT.Domain.Exceptions.Base;

namespace TE4IT.Domain.Exceptions.Tasks;

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
