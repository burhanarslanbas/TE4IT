namespace TE4IT.Domain.Events;

/// <summary>
/// Proje üyesi rolü değiştiğinde fırlatılan domain event
/// </summary>
public class ProjectMemberRoleChangedEvent : IDomainEvent
{
    public Guid EventId { get; }
    public DateTime OccurredAt { get; }
    public string EventType => nameof(ProjectMemberRoleChangedEvent);

    public Guid ProjectId { get; }
    public Guid UserId { get; }
    public int OldRole { get; } // ProjectRole enum value
    public int NewRole { get; } // ProjectRole enum value

    public ProjectMemberRoleChangedEvent(
        Guid projectId,
        Guid userId,
        int oldRole,
        int newRole)
    {
        EventId = Guid.NewGuid();
        OccurredAt = DateTime.UtcNow;

        ProjectId = projectId;
        UserId = userId;
        OldRole = oldRole;
        NewRole = newRole;
    }
}

