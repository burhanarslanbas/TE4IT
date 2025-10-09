using TE4IT.Domain.Enums;

namespace TE4IT.Domain.Events;

/// <summary>
/// Görev başlatıldığında fırlatılan domain event
/// </summary>
public class TaskStartedEvent : IDomainEvent
{
    public Guid EventId { get; }
    public DateTime OccurredAt { get; }
    public string EventType => nameof(TaskStartedEvent);

    public Guid TaskId { get; }
    public Guid UserId { get; }
    public Guid UseCaseId { get; }
    public string TaskTitle { get; }
    public TaskType TaskType { get; }
    public DateTime StartedAt { get; }

    public TaskStartedEvent(Guid taskId, Guid userId, Guid useCaseId, string taskTitle, TaskType taskType)
    {
        EventId = Guid.NewGuid();
        OccurredAt = DateTime.UtcNow;
        StartedAt = DateTime.UtcNow;

        TaskId = taskId;
        UserId = userId;
        UseCaseId = useCaseId;
        TaskTitle = taskTitle;
        TaskType = taskType;
    }
}


