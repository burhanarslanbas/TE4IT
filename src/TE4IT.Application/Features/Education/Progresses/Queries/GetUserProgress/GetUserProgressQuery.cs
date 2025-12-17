using MediatR;

namespace TE4IT.Application.Features.Education.Progresses.Queries.GetUserProgress;

public sealed record GetUserProgressQuery(Guid? CourseId = null) : IRequest<UserProgressResponse>;

