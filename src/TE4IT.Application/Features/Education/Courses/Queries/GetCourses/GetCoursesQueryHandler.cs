using MediatR;
using TE4IT.Application.Abstractions.Persistence.Repositories.Education.Courses;
using TE4IT.Application.Abstractions.Persistence.Repositories.Education.Enrollments;
using TE4IT.Application.Abstractions.Persistence.Repositories.Education.Enrollments;
using TE4IT.Application.Common.Pagination;
using TE4IT.Application.Features.Education.Courses.Responses;

namespace TE4IT.Application.Features.Education.Courses.Queries.GetCourses;

public sealed class GetCoursesQueryHandler(
    ICourseReadRepository courseReadRepository,
    IEnrollmentReadRepository enrollmentReadRepository) : IRequestHandler<GetCoursesQuery, PagedResult<CourseListItemResponse>>
    ICourseReadRepository courseReadRepository,
    IEnrollmentReadRepository enrollmentReadRepository) : IRequestHandler<GetCoursesQuery, PagedResult<CourseListItemResponse>>
{
    public async Task<PagedResult<CourseListItemResponse>> Handle(GetCoursesQuery request, CancellationToken cancellationToken)
    {
        var pagedCourses = await courseReadRepository.GetActiveAsync(
            request.Page,
            request.PageSize,
            request.Search,
            cancellationToken);

        var items = new List<CourseListItemResponse>();
        
        foreach (var course in pagedCourses.Items)
        {
            // Enrollment count'u her kurs için al
            var enrollmentCount = await enrollmentReadRepository.GetEnrollmentCountByCourseAsync(
                course.Id,
                cancellationToken);

            items.Add(new CourseListItemResponse
            {
                Id = course.Id,
                Title = course.Title,
                Description = course.Description,
                ThumbnailUrl = course.ThumbnailUrl,
                EstimatedDurationMinutes = course.Roadmap?.EstimatedDurationMinutes,
                StepCount = course.Roadmap?.Steps.Count,
                EnrollmentCount = enrollmentCount,
                CreatedAt = course.CreatedDate
            });
        }

        return new PagedResult<CourseListItemResponse>(
            items,
            pagedCourses.TotalCount,
            request.Page,
            request.PageSize);
    }
}

