using TE4IT.Domain.Enums;
using TE4IT.Domain.Exceptions.Base;

namespace TE4IT.Domain.Exceptions.Tasks;

/// <summary>
/// Geçersiz görev durumu geçişi için exception
/// </summary>
public class InvalidTaskStateTransitionException : DomainException
{
    public TaskState FromState { get; }
    public TaskState ToState { get; }

    public InvalidTaskStateTransitionException(TaskState fromState, TaskState toState)
        : base($"Invalid state transition from {fromState} to {toState}")
    {
        FromState = fromState;
        ToState = toState;
    }

    public InvalidTaskStateTransitionException(string message) : base(message)
    {
    }

    public InvalidTaskStateTransitionException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
