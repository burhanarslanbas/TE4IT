using MediatR;

namespace TE4IT.Application.Features.Education.Courses.Commands.CreateCourse;

public sealed record CreateCourseCommand(string Title, string Description, string? ThumbnailUrl) : IRequest<CreateCourseCommandResponse>;

