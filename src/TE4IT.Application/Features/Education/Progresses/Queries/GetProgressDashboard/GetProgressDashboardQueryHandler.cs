using MediatR;
using TE4IT.Application.Abstractions.Auth;
using TE4IT.Application.Abstractions.Persistence.Repositories.Education.Enrollments;
using TE4IT.Domain.Services;

namespace TE4IT.Application.Features.Education.Progresses.Queries.GetProgressDashboard;

public sealed class GetProgressDashboardQueryHandler(
    IEnrollmentReadRepository enrollmentReadRepository,
    ICourseProgressService courseProgressService,
    ICurrentUser currentUser) : IRequestHandler<GetProgressDashboardQuery, ProgressDashboardResponse>
{
    public async Task<ProgressDashboardResponse> Handle(GetProgressDashboardQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = currentUser.Id ?? throw new UnauthorizedAccessException("Kullanıcı kimliği bulunamadı.");

        // Tüm enrollments getirilir
        var enrollments = await enrollmentReadRepository.GetUserEnrollmentsAsync(
            currentUserId.Value,
            cancellationToken);

        var activeEnrollments = enrollments.Where(e => e.IsActive && e.CompletedAt is null).ToList();
        var completedEnrollments = enrollments.Where(e => e.CompletedAt is not null).ToList();

        // Her enrollment için progress yüzdesi hesaplanır
        var enrollmentItems = new List<EnrollmentProgressItem>();
        int totalTimeSpentMinutes = 0;

        foreach (var enrollment in enrollments)
        {
            var progressPercentage = await courseProgressService.CalculateProgressPercentageAsync(
                currentUserId.Value,
                enrollment.CourseId,
                cancellationToken);

            // TODO: Total time spent hesaplanacak (progress records'dan)
            enrollmentItems.Add(new EnrollmentProgressItem
            {
                EnrollmentId = enrollment.Id,
                CourseId = enrollment.CourseId,
                ProgressPercentage = progressPercentage,
                EnrolledAt = enrollment.EnrolledAt,
                StartedAt = enrollment.StartedAt,
                CompletedAt = enrollment.CompletedAt
            });
        }

        return new ProgressDashboardResponse
        {
            TotalCourses = enrollments.Count,
            ActiveCourses = activeEnrollments.Count,
            CompletedCourses = completedEnrollments.Count,
            TotalTimeSpentMinutes = totalTimeSpentMinutes,
            Enrollments = enrollmentItems
        };
    }
}

