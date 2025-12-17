using TE4IT.Domain.Entities.Common;
using TE4IT.Domain.Events;

namespace TE4IT.Domain.Entities.Education;

/// <summary>
/// Bir kullanıcının kursa kayıt durumunu temsil eden aggregate.
/// </summary>
public sealed class Enrollment : AggregateRoot
{
    private Enrollment()
    {
    }

    public Enrollment(Guid userId, Guid courseId)
    {
        UserId = userId;
        CourseId = courseId;
        EnrolledAt = DateTime.UtcNow;
        IsActive = true;
        
        // Domain event fırlat
        AddDomainEvent(new EnrollmentCreatedEvent(Id, userId, courseId));
    }

    public Guid UserId { get; private set; }
    public Guid CourseId { get; private set; }
    public DateTime EnrolledAt { get; private set; }
    public DateTime? StartedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public bool IsActive { get; private set; }

    public void MarkStarted()
    {
        if (StartedAt is null)
        {
            StartedAt = DateTime.UtcNow;
            UpdatedDate = StartedAt;
        }
    }

    public void MarkCompleted()
    {
        CompletedAt = DateTime.UtcNow;
        IsActive = false;
        UpdatedDate = CompletedAt;
        
        // Domain event fırlat
        AddDomainEvent(new CourseCompletedEvent(Id, UserId, CourseId));
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedDate = DateTime.UtcNow;
    }
}

