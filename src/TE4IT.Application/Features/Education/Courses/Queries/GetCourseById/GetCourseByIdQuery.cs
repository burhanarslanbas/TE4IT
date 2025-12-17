using MediatR;
using TE4IT.Application.Features.Education.Courses.Responses;

namespace TE4IT.Application.Features.Education.Courses.Queries.GetCourseById;

public sealed record GetCourseByIdQuery(Guid CourseId) : IRequest<CourseResponse?>;

