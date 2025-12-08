namespace TE4IT.Domain.Events;

/// <summary>
/// Proje daveti gönderildiğinde fırlatılan domain event
/// </summary>
public class ProjectInvitationSentEvent : IDomainEvent
{
    public Guid EventId { get; }
    public DateTime OccurredAt { get; }
    public string EventType => nameof(ProjectInvitationSentEvent);

    public Guid InvitationId { get; }
    public Guid ProjectId { get; }
    public string Email { get; }
    public int Role { get; }
    public Guid InvitedByUserId { get; }
    public DateTime ExpiresAt { get; }

    public ProjectInvitationSentEvent(
        Guid invitationId,
        Guid projectId,
        string email,
        int role,
        Guid invitedByUserId,
        DateTime expiresAt)
    {
        EventId = Guid.NewGuid();
        OccurredAt = DateTime.UtcNow;

        InvitationId = invitationId;
        ProjectId = projectId;
        Email = email;
        Role = role;
        InvitedByUserId = invitedByUserId;
        ExpiresAt = expiresAt;
    }
}

