using TE4IT.Domain.Entities.Common;

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

    public Guid UserId { get; private set; }
    public Guid EnrollmentId { get; private set; }
    public Guid CourseId { get; private set; }
    public Guid StepId { get; private set; }
    public Guid ContentId { get; private set; }
    public bool IsCompleted { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public int? TimeSpentMinutes { get; private set; }
    public DateTime? LastAccessedAt { get; private set; }
    public int? WatchedPercentage { get; private set; }

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
    }
}

