using MediatR;

namespace TE4IT.Application.Features.Education.Progresses.Queries.GetCourseProgress;

public sealed record GetCourseProgressQuery(Guid CourseId) : IRequest<CourseProgressResponse?>;

