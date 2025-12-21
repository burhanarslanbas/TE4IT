namespace TE4IT.Domain.Events;

/// <summary>
/// Kullanıcı kursa kayıt olduğunda fırlatılır.
/// </summary>
public sealed class EnrollmentCreatedEvent : IDomainEvent
{
    public EnrollmentCreatedEvent(Guid enrollmentId, Guid userId, Guid courseId)
    {
        EventId = Guid.NewGuid();
        OccurredAt = DateTime.UtcNow;
        EnrollmentId = enrollmentId;
        UserId = userId;
        CourseId = courseId;
    }

    public Guid EventId { get; }
    public DateTime OccurredAt { get; }
    public string EventType => nameof(EnrollmentCreatedEvent);

    public Guid EnrollmentId { get; }
    public Guid UserId { get; }
    public Guid CourseId { get; }
}

