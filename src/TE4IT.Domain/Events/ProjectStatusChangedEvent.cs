namespace TE4IT.Domain.Events;

/// <summary>
/// Proje durumu değiştiğinde fırlatılan domain event
/// </summary>
public class ProjectStatusChangedEvent : IDomainEvent
{
    public Guid EventId { get; }
    public DateTime OccurredAt { get; }
    public string EventType => nameof(ProjectStatusChangedEvent);

    public Guid ProjectId { get; }
    public Guid ChangedByUserId { get; }
    public string ProjectTitle { get; }
    public bool PreviousStatus { get; }
    public bool NewStatus { get; }
    public DateTime ChangedAt { get; }

    public ProjectStatusChangedEvent(
        Guid projectId,
        Guid changedByUserId,
        string projectTitle,
        bool previousStatus,
        bool newStatus)
    {
        EventId = Guid.NewGuid();
        OccurredAt = DateTime.UtcNow;
        ChangedAt = DateTime.UtcNow;

        ProjectId = projectId;
        ChangedByUserId = changedByUserId;
        ProjectTitle = projectTitle;
        PreviousStatus = previousStatus;
        NewStatus = newStatus;
    }
}
