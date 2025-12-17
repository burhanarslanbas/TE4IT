namespace TE4IT.Domain.Events;

/// <summary>
/// Proje üyesi çıkarıldığında fırlatılan domain event
/// </summary>
public class ProjectMemberRemovedEvent : IDomainEvent
{
    public Guid EventId { get; }
    public DateTime OccurredAt { get; }
    public string EventType => nameof(ProjectMemberRemovedEvent);

    public Guid ProjectId { get; }
    public Guid UserId { get; }

    public ProjectMemberRemovedEvent(
        Guid projectId,
        Guid userId)
    {
        EventId = Guid.NewGuid();
        OccurredAt = DateTime.UtcNow;

        ProjectId = projectId;
        UserId = userId;
    }
}

