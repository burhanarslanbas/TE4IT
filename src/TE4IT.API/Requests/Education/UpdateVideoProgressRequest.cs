namespace TE4IT.API.Requests.Education;

/// <summary>
/// Video ilerleme güncelleme request modeli
/// </summary>
public sealed record UpdateVideoProgressRequest(
    Guid CourseId,
    int WatchedPercentage,
    int TimeSpentSeconds,
    bool IsCompleted = false);

