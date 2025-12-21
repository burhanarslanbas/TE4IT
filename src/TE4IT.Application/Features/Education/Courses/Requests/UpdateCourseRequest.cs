namespace TE4IT.Application.Features.Education.Courses.Requests;

/// <summary>
/// Kurs güncelleme request DTO
/// </summary>
public record UpdateCourseRequest(string Title, string Description, string? ThumbnailUrl);

