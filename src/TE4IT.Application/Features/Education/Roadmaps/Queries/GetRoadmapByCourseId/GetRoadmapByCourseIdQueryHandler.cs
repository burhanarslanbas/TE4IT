using MediatR;
using TE4IT.Application.Abstractions.Persistence.Repositories.Education.Courses;
using TE4IT.Application.Features.Education.Courses.Responses;
using TE4IT.Application.Features.Education.Roadmaps.Responses;
using TE4IT.Domain.Enums.Education;
using TE4IT.Domain.Exceptions.Common;
using TE4IT.Domain.Services;

namespace TE4IT.Application.Features.Education.Roadmaps.Queries.GetRoadmapByCourseId;

public sealed class GetRoadmapByCourseIdQueryHandler(
    ICourseReadRepository courseReadRepository,
    IVideoUrlService videoUrlService) : IRequestHandler<GetRoadmapByCourseIdQuery, RoadmapResponse?>
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
                IsRequired = step.IsRequired,
                EstimatedDurationMinutes = step.EstimatedDurationMinutes,
                Contents = step.Contents.Select(content =>
                {
                    // Video içeriklerine embedUrl ve platform ekle
                    string? embedUrl = null;
                    string? platform = null;
                    if (content.Type == ContentType.VideoLink && !string.IsNullOrEmpty(content.LinkUrl))
                    {
                        embedUrl = videoUrlService.GetEmbedUrl(content.LinkUrl);
                        platform = videoUrlService.DetectPlatform(content.LinkUrl);
                    }

                    return new ContentResponse
                    {
                        Id = content.Id,
                        Title = content.Title,
                        Description = content.Description,
                        Type = content.Type,
                        Order = content.Order,
                        IsRequired = content.IsRequired,
                        Content = content.Content,
                        LinkUrl = content.LinkUrl,
                        EmbedUrl = embedUrl ?? content.EmbedUrl,
                        Platform = platform ?? content.Platform
                    };
                }).ToList()
            }).ToList()
        };
    }
}

