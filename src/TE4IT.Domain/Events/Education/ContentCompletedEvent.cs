namespace TE4IT.Domain.Events;

/// <summary>
/// Kullanıcı bir içeriği tamamladığında fırlatılır.
/// </summary>
public sealed class ContentCompletedEvent : IDomainEvent
{
    public ContentCompletedEvent(Guid progressId, Guid userId, Guid contentId)
    {
        EventId = Guid.NewGuid();
        OccurredAt = DateTime.UtcNow;
        ProgressId = progressId;
        UserId = userId;
        ContentId = contentId;
    }

    public Guid EventId { get; }
    public DateTime OccurredAt { get; }
    public string EventType => nameof(ContentCompletedEvent);

    public Guid ProgressId { get; }
    public Guid UserId { get; }
    public Guid ContentId { get; }
}

