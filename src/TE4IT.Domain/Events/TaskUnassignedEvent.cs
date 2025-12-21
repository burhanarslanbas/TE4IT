using TE4IT.Domain.Enums;

namespace TE4IT.Domain.Events;

/// <summary>
/// Görevden atama kaldırıldığında fırlatılan domain event
/// </summary>
public class TaskUnassignedEvent : IDomainEvent
{
    public Guid EventId { get; }
    public DateTime OccurredAt { get; }
    public string EventType => nameof(TaskUnassignedEvent);

    public Guid TaskId { get; }
    public Guid UseCaseId { get; }
    public string TaskTitle { get; }
    public TaskType TaskType { get; }

    public TaskUnassignedEvent(
        Guid taskId,
        Guid useCaseId,
        string taskTitle,
        TaskType taskType)
    {
        EventId = Guid.NewGuid();
        OccurredAt = DateTime.UtcNow;

        TaskId = taskId;
        UseCaseId = useCaseId;
        TaskTitle = taskTitle;
        TaskType = taskType;
    }
}

