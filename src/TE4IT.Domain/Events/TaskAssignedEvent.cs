using TE4IT.Domain.Enums;

namespace TE4IT.Domain.Events;

/// <summary>
/// Görev atandığında fırlatılan domain event
/// </summary>
public class TaskAssignedEvent : IDomainEvent
{
    public Guid EventId { get; }
    public DateTime OccurredAt { get; }
    public string EventType => nameof(TaskAssignedEvent);

    public Guid TaskId { get; }
    public Guid AssigneeId { get; }
    public Guid AssignerId { get; }
    public Guid UseCaseId { get; }
    public string TaskTitle { get; }
    public TaskType TaskType { get; }
    public DateTime? DueDate { get; }
    public DateTime AssignedAt { get; }

    public TaskAssignedEvent(
        Guid taskId,
        Guid assigneeId,
        Guid assignerId,
        Guid useCaseId,
        string taskTitle,
        TaskType taskType,
        DateTime? dueDate)
    {
        EventId = Guid.NewGuid();
        OccurredAt = DateTime.UtcNow;
        AssignedAt = DateTime.UtcNow;

        TaskId = taskId;
        AssigneeId = assigneeId;
        AssignerId = assignerId;
        UseCaseId = useCaseId;
        TaskTitle = taskTitle;
        TaskType = taskType;
        DueDate = dueDate;
    }
}
