using MediatR;
using TE4IT.Application.Common.Pagination;
using TE4IT.Application.Features.Education.Courses.Responses;

namespace TE4IT.Application.Features.Education.Courses.Queries.GetCourses;

public sealed record GetCoursesQuery(
    int Page = 1,
    int PageSize = 10,
    string? Search = null) : IRequest<PagedResult<CourseListItemResponse>>;

