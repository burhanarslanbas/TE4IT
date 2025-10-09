using TE4IT.Domain.Enums;

namespace TE4IT.Domain.Events;

/// <summary>
/// Görev iptal edildiğinde fırlatılan domain event
/// </summary>
public class TaskCancelledEvent : IDomainEvent
{
    public Guid EventId { get; }
    public DateTime OccurredAt { get; }
    public string EventType => nameof(TaskCancelledEvent);

    public Guid TaskId { get; }
    public Guid UserId { get; }
    public Guid UseCaseId { get; }
    public string TaskTitle { get; }
    public TaskType TaskType { get; }
    public DateTime CancelledAt { get; }

    public TaskCancelledEvent(Guid taskId, Guid userId, Guid useCaseId, string taskTitle, TaskType taskType)
    {
        EventId = Guid.NewGuid();
        OccurredAt = DateTime.UtcNow;
        CancelledAt = DateTime.UtcNow;

        TaskId = taskId;
        UserId = userId;
        UseCaseId = useCaseId;
        TaskTitle = taskTitle;
        TaskType = taskType;
    }
}


