using MediatR;
using TE4IT.Application.Abstractions.Auth;
using TE4IT.Application.Abstractions.Persistence.Repositories.Education.Courses;
using TE4IT.Application.Abstractions.Persistence.Repositories.Education.Enrollments;
using TE4IT.Application.Abstractions.Persistence.Repositories.Education.Progresses;
using TE4IT.Domain.Exceptions.Common;
using TE4IT.Domain.Services;

namespace TE4IT.Application.Features.Education.Progresses.Queries.GetCourseProgress;

public sealed class GetCourseProgressQueryHandler(
    ICourseReadRepository courseReadRepository,
    IEnrollmentReadRepository enrollmentReadRepository,
    IProgressReadRepository progressReadRepository,
    ICourseProgressService courseProgressService,
    ICurrentUser currentUser) : IRequestHandler<GetCourseProgressQuery, CourseProgressResponse?>
{
    public async Task<CourseProgressResponse?> Handle(GetCourseProgressQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = currentUser.Id ?? throw new UnauthorizedAccessException("Kullanıcı kimliği bulunamadı.");

        // Course + Roadmap getirilir
        var course = await courseReadRepository.GetByIdAsync(request.CourseId, cancellationToken);
        if (course is null)
        {
            throw new ResourceNotFoundException("Course", request.CourseId);
        }

        // Enrollment kontrolü
        var enrollment = await enrollmentReadRepository.GetByUserAndCourseAsync(
            currentUserId.Value,
            request.CourseId,
            cancellationToken);

        if (enrollment is null)
        {
            throw new BusinessRuleViolationException("Bu kursa kayıtlı değilsiniz.");
        }

        // Progress kayıtları getirilir
        var progressRecords = await progressReadRepository.GetUserProgressByCourseAsync(
            currentUserId.Value,
            request.CourseId,
            cancellationToken);

        // Progress yüzdesi hesaplanır
        var progressPercentage = await courseProgressService.CalculateProgressPercentageAsync(
            currentUserId.Value,
            request.CourseId,
            cancellationToken);

        // Her step için tamamlanma durumu hesaplanır
        var stepProgresses = new List<StepProgressItem>();
        if (course.Roadmap is not null)
        {
            foreach (var step in course.Roadmap.Steps)
            {
                var stepCompletedCount = await progressReadRepository.GetCompletedContentCountAsync(
                    currentUserId.Value,
                    step.Id,
                    request.CourseId,
                    cancellationToken);

                var stepTotalCount = step.Contents.Count;
                var stepProgressPercentage = stepTotalCount > 0
                    ? (decimal)stepCompletedCount / stepTotalCount * 100
                    : 0;

                var contentProgresses = step.Contents.Select(content =>
                {
                    var contentProgress = progressRecords.FirstOrDefault(p => p.ContentId == content.Id);
                    return new ContentProgressItem
                    {
                        ContentId = content.Id,
                        Title = content.Title,
                        IsCompleted = contentProgress?.IsCompleted ?? false,
                        CompletedAt = contentProgress?.CompletedAt,
                        TimeSpentMinutes = contentProgress?.TimeSpentMinutes,
                        WatchedPercentage = contentProgress?.WatchedPercentage
                    };
                }).ToList();

                stepProgresses.Add(new StepProgressItem
                {
                    StepId = step.Id,
                    Title = step.Title,
                    Order = step.Order,
                    ProgressPercentage = stepProgressPercentage,
                    CompletedContentCount = stepCompletedCount,
                    TotalContentCount = stepTotalCount,
                    Contents = contentProgresses
                });
            }
        }

        return new CourseProgressResponse
        {
            CourseId = request.CourseId,
            CourseTitle = course.Title,
            ProgressPercentage = progressPercentage,
            Steps = stepProgresses
        };
    }
}

