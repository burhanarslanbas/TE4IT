using MediatR;
using TE4IT.Application.Features.Education.Roadmaps.Responses;

namespace TE4IT.Application.Features.Education.Roadmaps.Queries.GetRoadmapByCourseId;

public sealed record GetRoadmapByCourseIdQuery(Guid CourseId) : IRequest<RoadmapResponse?>;

