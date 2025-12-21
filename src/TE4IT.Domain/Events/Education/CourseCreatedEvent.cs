namespace TE4IT.Domain.Events;

/// <summary>
/// Yeni kurs oluşturulduğunda fırlatılır.
/// </summary>
public sealed class CourseCreatedEvent : IDomainEvent
{
    public CourseCreatedEvent(Guid courseId, Guid createdBy, string title)
    {
        EventId = Guid.NewGuid();
        OccurredAt = DateTime.UtcNow;
        CourseId = courseId;
        CreatedBy = createdBy;
        Title = title;
    }

    public Guid EventId { get; }
    public DateTime OccurredAt { get; }
    public string EventType => nameof(CourseCreatedEvent);

    public Guid CourseId { get; }
    public Guid CreatedBy { get; }
    public string Title { get; }
}

