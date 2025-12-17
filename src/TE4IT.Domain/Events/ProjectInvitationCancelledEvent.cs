namespace TE4IT.Domain.Events;

/// <summary>
/// Proje daveti iptal edildiğinde fırlatılan domain event
/// </summary>
public class ProjectInvitationCancelledEvent : IDomainEvent
{
    public Guid EventId { get; }
    public DateTime OccurredAt { get; }
    public string EventType => nameof(ProjectInvitationCancelledEvent);

    public Guid InvitationId { get; }
    public Guid ProjectId { get; }
    public string Email { get; }
    public DateTime CancelledAt { get; }

    public ProjectInvitationCancelledEvent(
        Guid invitationId,
        Guid projectId,
        string email,
        DateTime cancelledAt)
    {
        EventId = Guid.NewGuid();
        OccurredAt = DateTime.UtcNow;

        InvitationId = invitationId;
        ProjectId = projectId;
        Email = email;
        CancelledAt = cancelledAt;
    }
}

