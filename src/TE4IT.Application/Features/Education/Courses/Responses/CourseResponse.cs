using TE4IT.Application.Features.Education.Enrollments.Responses;
using TE4IT.Application.Features.Education.Roadmaps.Responses;

namespace TE4IT.Application.Features.Education.Courses.Responses;

/// <summary>
/// Kurs detayı için response DTO
/// </summary>
public sealed class CourseResponse
{
    public Guid Id { get; init; }
    public string Title { get; init; } = default!;
    public string Description { get; init; } = default!;
    public string? ThumbnailUrl { get; init; }
    public int? EstimatedDurationMinutes { get; init; }
    public int StepCount { get; init; }
    public int EnrollmentCount { get; init; }
    public DateTime CreatedAt { get; init; }
    public RoadmapResponse? Roadmap { get; init; }
    public EnrollmentResponse? UserEnrollment { get; init; }
    public decimal ProgressPercentage { get; init; }
}