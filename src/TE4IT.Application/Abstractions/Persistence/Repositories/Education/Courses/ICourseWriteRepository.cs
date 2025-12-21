using TE4IT.Application.Abstractions.Persistence.Repositories.Base;
using TE4IT.Domain.Entities.Education;

namespace TE4IT.Application.Abstractions.Persistence.Repositories.Education.Courses;

public interface ICourseWriteRepository : IWriteRepository<Course>
{
    Task<bool> SoftDeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

