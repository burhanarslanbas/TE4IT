using MediatR;
using TE4IT.Application.Abstractions.Auth;
using TE4IT.Application.Abstractions.Persistence.Repositories.Education.Courses;
using TE4IT.Application.Abstractions.Persistence.Repositories.Education.Enrollments;
using TE4IT.Application.Abstractions.Persistence.Repositories.Education.Progresses;

namespace TE4IT.Application.Features.Education.Enrollments.Queries.GetUserEnrollments;

public sealed class GetUserEnrollmentsQueryHandler(
    IEnrollmentReadRepository enrollmentReadRepository,
    ICourseReadRepository courseReadRepository,
    IProgressReadRepository progressReadRepository,
    ICurrentUser currentUser) : IRequestHandler<GetUserEnrollmentsQuery, IReadOnlyList<EnrollmentListItemResponse>>
{
    public async Task<IReadOnlyList<EnrollmentListItemResponse>> Handle(GetUserEnrollmentsQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = currentUser.Id ?? throw new UnauthorizedAccessException("Kullanıcı kimliği bulunamadı.");

        var enrollments = await enrollmentReadRepository.GetUserEnrollmentsAsync(currentUserId.Value, cancellationToken);

        // Status filtresi uygulanır
        var filteredEnrollments = request.Status switch
        {
            "active" => enrollments.Where(e => e.IsActive && e.CompletedAt is null).ToList(),
            "completed" => enrollments.Where(e => e.CompletedAt is not null).ToList(),
            _ => enrollments.ToList()
        };

        var result = new List<EnrollmentListItemResponse>();

        foreach (var enrollment in filteredEnrollments)
        {
            var course = await courseReadRepository.GetByIdAsync(enrollment.CourseId, cancellationToken);
            if (course is null) continue;

            // TODO: ICourseProgressService ile progress yüzdesi hesaplanacak
            // Şimdilik basit bir hesaplama yapıyoruz
            var totalContentCount = course.Roadmap?.Steps
                .SelectMany(s => s.Contents)
                .Count() ?? 0;

            var completedContentCount = 0;
            if (course.Roadmap is not null)
            {
                foreach (var step in course.Roadmap.Steps)
                {
                    var stepCompletedCount = await progressReadRepository.GetCompletedContentCountAsync(
                        currentUserId.Value,
                        step.Id,
                        enrollment.CourseId,
                        cancellationToken);
                    completedContentCount += stepCompletedCount;
                }
            }

            var progressPercentage = totalContentCount > 0
                ? (decimal)completedContentCount / totalContentCount * 100
                : 0;

            result.Add(new EnrollmentListItemResponse
            {
                Id = enrollment.Id,
                CourseId = enrollment.CourseId,
                CourseTitle = course.Title,
                CourseDescription = course.Description,
                ThumbnailUrl = course.ThumbnailUrl,
                EnrolledAt = enrollment.EnrolledAt,
                StartedAt = enrollment.StartedAt,
                CompletedAt = enrollment.CompletedAt,
                IsActive = enrollment.IsActive,
                ProgressPercentage = progressPercentage
            });
        }

        return result;
    }
}

