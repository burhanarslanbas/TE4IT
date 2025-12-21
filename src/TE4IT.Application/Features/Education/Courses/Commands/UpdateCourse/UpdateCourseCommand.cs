using MediatR;

namespace TE4IT.Application.Features.Education.Courses.Commands.UpdateCourse;

public sealed record UpdateCourseCommand(Guid CourseId, string Title, string Description, string? ThumbnailUrl) : IRequest<bool>;

