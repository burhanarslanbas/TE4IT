using TE4IT.Domain.Enums;

namespace TE4IT.Domain.Events;

/// <summary>
/// Görev tamamlandığında fırlatılan domain event
/// </summary>
public class TaskCompletedEvent : IDomainEvent
{
    public Guid EventId { get; }
    public DateTime OccurredAt { get; }
    public string EventType => nameof(TaskCompletedEvent);

    public Guid TaskId { get; }
    public Guid UserId { get; }
    public Guid UseCaseId { get; }
    public string TaskTitle { get; }
    public TaskType TaskType { get; }
    public string? CompletionNote { get; }
    public DateTime CompletedAt { get; }

    public TaskCompletedEvent(
        Guid taskId,
        Guid userId,
        Guid useCaseId,
        string taskTitle,
        TaskType taskType,
        string? completionNote = null)
    {
        EventId = Guid.NewGuid();
        OccurredAt = DateTime.UtcNow;
        CompletedAt = DateTime.UtcNow;

        TaskId = taskId;
        UserId = userId;
        UseCaseId = useCaseId;
        TaskTitle = taskTitle;
        TaskType = taskType;
        CompletionNote = completionNote;
    }
}
