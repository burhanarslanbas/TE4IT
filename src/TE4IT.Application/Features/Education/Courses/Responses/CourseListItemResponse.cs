namespace TE4IT.Application.Features.Education.Courses.Responses;

/// <summary>
/// Kurs listesi için response DTO
/// </summary>
public sealed class CourseListItemResponse
{
    public Guid Id { get; init; }
    public string Title { get; init; } = default!;
    public string Description { get; init; } = default!;
    public string? ThumbnailUrl { get; init; }
    public int? EstimatedDurationMinutes { get; init; }
    public int? StepCount { get; init; }
    public int EnrollmentCount { get; init; }
    public DateTime CreatedAt { get; init; }
}