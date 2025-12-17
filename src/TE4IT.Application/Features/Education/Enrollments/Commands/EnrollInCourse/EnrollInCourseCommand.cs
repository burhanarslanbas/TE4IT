using MediatR;

namespace TE4IT.Application.Features.Education.Enrollments.Commands.EnrollInCourse;

public sealed record EnrollInCourseCommand(Guid CourseId) : IRequest<EnrollInCourseCommandResponse>;

