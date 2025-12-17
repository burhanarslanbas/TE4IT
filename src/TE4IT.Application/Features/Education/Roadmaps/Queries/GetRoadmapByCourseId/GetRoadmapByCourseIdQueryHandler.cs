using MediatR;
using TE4IT.Application.Abstractions.Persistence.Repositories.Education.Courses;
using TE4IT.Application.Features.Education.Courses.Responses;
using TE4IT.Application.Features.Education.Roadmaps.Responses;
using TE4IT.Domain.Exceptions.Common;

namespace TE4IT.Application.Features.Education.Roadmaps.Queries.GetRoadmapByCourseId;

public sealed class GetRoadmapByCourseIdQueryHandler(
    ICourseReadRepository courseReadRepository) : IRequestHandler<GetRoadmapByCourseIdQuery, RoadmapResponse?>
{
    public async Task<RoadmapResponse?> Handle(GetRoadmapByCourseIdQuery request, CancellationToken cancellationToken)
    {
        var course = await courseReadRepository.GetByIdAsync(request.CourseId, cancellationToken);
        if (course is null)
        {
            throw new ResourceNotFoundException("Course", request.CourseId);
        }

        if (course.Roadmap is null)
        {
            return null;
        }

        // TODO: IVideoUrlService ile video içeriklerine embedUrl eklenecek

        return new RoadmapResponse
        {
            Title = course.Roadmap.Title,
            Description = course.Roadmap.Description,
            EstimatedDurationMinutes = course.Roadmap.EstimatedDurationMinutes,
            Steps = course.Roadmap.Steps.Select(step => new StepResponse
            {
                Id = step.Id,
                Title = step.Title,
                Description = step.Description,
                Order = step.Order,
                Contents = step.Contents.Select(content => new ContentResponse
                {
                    Id = content.Id,
                    Title = content.Title,
                    Description = content.Description,
                    Type = content.Type,
                    Content = content.Content,
                    LinkUrl = content.LinkUrl,
                    EmbedUrl = content.EmbedUrl
                }).ToList()
            }).ToList()
        };
    }
}

