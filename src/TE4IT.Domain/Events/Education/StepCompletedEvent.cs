namespace TE4IT.Domain.Events;

/// <summary>
/// Bir roadmap adımı tamamlandığında fırlatılır.
/// </summary>
public sealed class StepCompletedEvent : IDomainEvent
{
    public StepCompletedEvent(Guid userId, Guid stepId, Guid courseId)
    {
        EventId = Guid.NewGuid();
        OccurredAt = DateTime.UtcNow;
        UserId = userId;
        StepId = stepId;
        CourseId = courseId;
    }

    public Guid EventId { get; }
    public DateTime OccurredAt { get; }
    public string EventType => nameof(StepCompletedEvent);

    public Guid UserId { get; }
    public Guid StepId { get; }
    public Guid CourseId { get; }
}

