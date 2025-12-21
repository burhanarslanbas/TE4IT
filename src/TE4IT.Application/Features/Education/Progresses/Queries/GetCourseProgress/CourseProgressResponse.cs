namespace TE4IT.Application.Features.Education.Progresses.Queries.GetCourseProgress;

/// <summary>
/// Kurs ilerleme bilgisi için response DTO
/// </summary>
public sealed class CourseProgressResponse
{
    public Guid CourseId { get; init; }
    public string CourseTitle { get; init; } = default!;
    public decimal ProgressPercentage { get; init; }
    public IReadOnlyList<StepProgressItem> Steps { get; init; } = Array.Empty<StepProgressItem>();
}

public sealed class StepProgressItem
{
    public Guid StepId { get; init; }
    public string Title { get; init; } = default!;
    public int Order { get; init; }
    public decimal ProgressPercentage { get; init; }
    public int CompletedContentCount { get; init; }
    public int TotalContentCount { get; init; }
    public IReadOnlyList<ContentProgressItem> Contents { get; init; } = Array.Empty<ContentProgressItem>();
}

public sealed class ContentProgressItem
{
    public Guid ContentId { get; init; }
    public string Title { get; init; } = default!;
    public bool IsCompleted { get; init; }
    public DateTime? CompletedAt { get; init; }
    public int? TimeSpentMinutes { get; init; }
    public int? WatchedPercentage { get; init; }
}

