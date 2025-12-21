using TE4IT.Application.Abstractions.Persistence.Repositories.Base;
using TE4IT.Application.Common.Pagination;
using TE4IT.Domain.Entities.Education;

namespace TE4IT.Application.Abstractions.Persistence.Repositories.Education.Courses;

public interface ICourseReadRepository : IReadRepository<Course>
{
    Task<PagedResult<Course>> GetActiveAsync(int page, int pageSize, string? search, CancellationToken cancellationToken = default);
}

