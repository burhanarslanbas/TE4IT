namespace TE4IT.Domain.Events;

/// <summary>
/// Kurs tüm zorunlu adımlar tamamlandığında fırlatılır.
/// </summary>
public sealed class CourseCompletedEvent : IDomainEvent
{
    public CourseCompletedEvent(Guid enrollmentId, Guid userId, Guid courseId)
    {
        EventId = Guid.NewGuid();
        OccurredAt = DateTime.UtcNow;
        EnrollmentId = enrollmentId;
        UserId = userId;
        CourseId = courseId;
    }

    public Guid EventId { get; }
    public DateTime OccurredAt { get; }
    public string EventType => nameof(CourseCompletedEvent);

    public Guid EnrollmentId { get; }
    public Guid UserId { get; }
    public Guid CourseId { get; }
}

