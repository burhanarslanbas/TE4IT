namespace TE4IT.Application.Features.Education.Enrollments.Queries.GetUserEnrollments;

/// <summary>
/// Kullanıcı kayıt listesi için response DTO
/// </summary>
public sealed class EnrollmentListItemResponse
{
    public Guid Id { get; init; }
    public Guid CourseId { get; init; }
    public string CourseTitle { get; init; } = default!;
    public string CourseDescription { get; init; } = default!;
    public string? ThumbnailUrl { get; init; }
    public DateTime EnrolledAt { get; init; }
    public DateTime? StartedAt { get; init; }
    public DateTime? CompletedAt { get; init; }
    public bool IsActive { get; init; }
    public decimal ProgressPercentage { get; init; }
}

