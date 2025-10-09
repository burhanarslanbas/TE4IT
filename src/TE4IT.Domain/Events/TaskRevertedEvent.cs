using TE4IT.Domain.Enums;

namespace TE4IT.Domain.Events;

/// <summary>
/// Görev NotStarted durumuna geri alındığında fırlatılan domain event
/// </summary>
public class TaskRevertedEvent : IDomainEvent
{
    public Guid EventId { get; }
    public DateTime OccurredAt { get; }
    public string EventType => nameof(TaskRevertedEvent);

    public Guid TaskId { get; }
    public Guid UserId { get; }
    public Guid UseCaseId { get; }
    public string TaskTitle { get; }
    public TaskType TaskType { get; }
    public DateTime RevertedAt { get; }

    public TaskRevertedEvent(Guid taskId, Guid userId, Guid useCaseId, string taskTitle, TaskType taskType)
    {
        EventId = Guid.NewGuid();
        OccurredAt = DateTime.UtcNow;
        RevertedAt = DateTime.UtcNow;

        TaskId = taskId;
        UserId = userId;
        UseCaseId = useCaseId;
        TaskTitle = taskTitle;
        TaskType = taskType;
    }
}


