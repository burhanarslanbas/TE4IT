namespace TE4IT.API.Requests.Education;

/// <summary>
/// İçerik tamamlama request modeli
/// </summary>
public sealed record CompleteContentRequest(
    Guid CourseId,
    int? TimeSpentMinutes = null,
    int? WatchedPercentage = null);

