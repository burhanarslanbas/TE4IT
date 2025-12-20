using TE4IT.Domain.Entities.Common;
using TE4IT.Domain.Events;

namespace TE4IT.Domain.Entities.Education;

/// <summary>
/// Kullanıcı bazlı içerik ilerleme kaydı. Yüksek hacimli olduğu için ayrı aggregate olarak tutulur.
/// </summary>
public sealed class Progress : AggregateRoot
{
    private Progress()
    {
    }

    public Progress(Guid userId, Guid enrollmentId, Guid courseId, Guid stepId, Guid contentId)
    {
        UserId = userId;
        EnrollmentId = enrollmentId;
        CourseId = courseId;
        StepId = stepId;
        ContentId = contentId;
        LastAccessedAt = DateTime.UtcNow;
    }

    public Guid UserId { get; set; }
    public Guid EnrollmentId { get; set; }
    public Guid CourseId { get; set; }
    public Guid StepId { get; set; }
    public Guid ContentId { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime? CompletedAt { get; set; }
    public int? TimeSpentMinutes { get; set; }
    public DateTime? LastAccessedAt { get; set; }
    public int? WatchedPercentage { get; set; }

    public void MarkAccessed()
    {
        LastAccessedAt = DateTime.UtcNow;
    }

    public void UpdateWatchedPercentage(int? watchedPercentage)
    {
        WatchedPercentage = watchedPercentage;
        LastAccessedAt = DateTime.UtcNow;
    }

    public void MarkCompleted(int? timeSpentMinutes = null, int? watchedPercentage = null)
    {
        IsCompleted = true;
        CompletedAt = DateTime.UtcNow;
        TimeSpentMinutes = timeSpentMinutes;
        WatchedPercentage = watchedPercentage ?? WatchedPercentage;
        UpdatedDate = CompletedAt;
        
        // Domain event fırlat
        AddDomainEvent(new ContentCompletedEvent(Id, UserId, ContentId));
    }
}

