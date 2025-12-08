namespace TE4IT.Domain.Events;

/// <summary>
/// Proje üyesi eklendiğinde fırlatılan domain event
/// </summary>
public class ProjectMemberAddedEvent : IDomainEvent
{
    public Guid EventId { get; }
    public DateTime OccurredAt { get; }
    public string EventType => nameof(ProjectMemberAddedEvent);

    public Guid ProjectId { get; }
    public Guid UserId { get; }
    public int Role { get; } // ProjectRole enum value
    public DateTime JoinedDate { get; }

    public ProjectMemberAddedEvent(
        Guid projectId,
        Guid userId,
        int role,
        DateTime joinedDate)
    {
        EventId = Guid.NewGuid();
        OccurredAt = DateTime.UtcNow;

        ProjectId = projectId;
        UserId = userId;
        Role = role;
        JoinedDate = joinedDate;
    }
}

