namespace TE4IT.Application.Features.Education.Progresses.Queries.GetUserProgress;

/// <summary>
/// Kullanıcı ilerleme bilgisi için response DTO
/// </summary>
public sealed class UserProgressResponse
{
    public Guid UserId { get; init; }
    public IReadOnlyList<CourseProgressItem> CourseProgresses { get; init; } = Array.Empty<CourseProgressItem>();
}

public sealed class CourseProgressItem
{
    public Guid CourseId { get; init; }
    public string CourseTitle { get; init; } = default!;
    public decimal ProgressPercentage { get; init; }
    public int CompletedContentCount { get; init; }
    public int TotalContentCount { get; init; }
}

