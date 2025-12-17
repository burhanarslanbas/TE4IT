using MediatR;

namespace TE4IT.Application.Features.Education.Courses.Commands.DeleteCourse;

public sealed record DeleteCourseCommand(Guid CourseId) : IRequest<bool>;

