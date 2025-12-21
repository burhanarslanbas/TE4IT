namespace TE4IT.Application.Features.Education.Progresses.Queries.GetProgressDashboard;

/// <summary>
/// İlerleme dashboard'u için response DTO
/// </summary>
public sealed class ProgressDashboardResponse
{
    public int TotalCourses { get; init; }
    public int ActiveCourses { get; init; }
    public int CompletedCourses { get; init; }
    public int TotalTimeSpentMinutes { get; init; }
    public IReadOnlyList<EnrollmentProgressItem> Enrollments { get; init; } = Array.Empty<EnrollmentProgressItem>();
}

public sealed class EnrollmentProgressItem
{
    public Guid EnrollmentId { get; init; }
    public Guid CourseId { get; init; }
    public decimal ProgressPercentage { get; init; }
    public DateTime EnrolledAt { get; init; }
    public DateTime? StartedAt { get; init; }
    public DateTime? CompletedAt { get; init; }
}

