using MediatR;
using TE4IT.Application.Abstractions.Auth;
using TE4IT.Application.Abstractions.Persistence.Repositories.Education.Courses;
using TE4IT.Application.Abstractions.Persistence.Repositories.Education.Enrollments;
using TE4IT.Application.Features.Education.Courses.Responses;
using TE4IT.Domain.Entities.Education;
using TE4IT.Application.Features.Education.Enrollments.Responses;
using TE4IT.Application.Features.Education.Roadmaps.Responses;
using TE4IT.Domain.Enums.Education;
using TE4IT.Domain.Services;

namespace TE4IT.Application.Features.Education.Courses.Queries.GetCourseById;

public sealed class GetCourseByIdQueryHandler(
    ICourseReadRepository courseReadRepository,
    IEnrollmentReadRepository enrollmentReadRepository,
    ICourseProgressService courseProgressService,
    IVideoUrlService videoUrlService,
    ICurrentUser currentUser) : IRequestHandler<GetCourseByIdQuery, CourseResponse?>
{
    public async Task<CourseResponse?> Handle(GetCourseByIdQuery request, CancellationToken cancellationToken)
    {
        var course = await courseReadRepository.GetByIdAsync(request.CourseId, cancellationToken);
        if (course is null)
        {
            return null;
        }

        var currentUserId = currentUser.Id;
        Enrollment? userEnrollment = null;
        if (currentUserId is not null)
        {
            userEnrollment = await enrollmentReadRepository.GetByUserAndCourseAsync(
                currentUserId.Value,
                request.CourseId,
                cancellationToken);
        }

        // Progress hesaplama
        var progressPercentage = currentUserId is not null
            ? await courseProgressService.CalculateProgressPercentageAsync(
                currentUserId.Value,
                course.Id,
                cancellationToken)
            : 0;

        // Enrollment count hesaplama
        var enrollmentCount = await enrollmentReadRepository.GetEnrollmentCountByCourseAsync(
            course.Id,
            cancellationToken);

        // Roadmap response oluşturma (video içeriklerine embedUrl ve platform ekle)
        RoadmapResponse? roadmapResponse = null;
        if (course.Roadmap is not null)
        {
            roadmapResponse = new RoadmapResponse
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
                            Content = content.Content,
                            LinkUrl = content.LinkUrl,
                            EmbedUrl = embedUrl,
                            Platform = platform
                        };
                    }).ToList()
                }).ToList()
            };
        }

        return new CourseResponse
        {
            Id = course.Id,
            Title = course.Title,
            Description = course.Description,
            ThumbnailUrl = course.ThumbnailUrl,
            EstimatedDurationMinutes = course.Roadmap?.EstimatedDurationMinutes,
            StepCount = course.Roadmap?.Steps.Count ?? 0,
            EnrollmentCount = enrollmentCount,
            CreatedAt = course.CreatedDate,
            Roadmap = roadmapResponse,
            UserEnrollment = userEnrollment is not null ? new EnrollmentResponse
            {
                Id = userEnrollment.Id,
                EnrolledAt = userEnrollment.EnrolledAt,
                StartedAt = userEnrollment.StartedAt,
                CompletedAt = userEnrollment.CompletedAt,
                IsActive = userEnrollment.IsActive
            } : null,
            ProgressPercentage = progressPercentage
        };
    }
}

