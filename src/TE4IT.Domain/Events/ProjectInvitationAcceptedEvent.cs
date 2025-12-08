namespace TE4IT.Domain.Events;

/// <summary>
/// Proje daveti kabul edildiğinde fırlatılan domain event
/// </summary>
public class ProjectInvitationAcceptedEvent : IDomainEvent
{
    public Guid EventId { get; }
    public DateTime OccurredAt { get; }
    public string EventType => nameof(ProjectInvitationAcceptedEvent);

    public Guid InvitationId { get; }
    public Guid ProjectId { get; }
    public string Email { get; }
    public Guid AcceptedByUserId { get; }
    public DateTime AcceptedAt { get; }

    public ProjectInvitationAcceptedEvent(
        Guid invitationId,
        Guid projectId,
        string email,
        Guid acceptedByUserId,
        DateTime acceptedAt)
    {
        EventId = Guid.NewGuid();
        OccurredAt = DateTime.UtcNow;

        InvitationId = invitationId;
        ProjectId = projectId;
        Email = email;
        AcceptedByUserId = acceptedByUserId;
        AcceptedAt = acceptedAt;
    }
}

