using TE4IT.Application.Abstractions.Persistence.Repositories.Base;
using TE4IT.Domain.Entities.Education;

namespace TE4IT.Application.Abstractions.Persistence.Repositories.Education.Enrollments;

public interface IEnrollmentReadRepository : IReadRepository<Enrollment>
{
    Task<Enrollment?> GetByUserAndCourseAsync(Guid userId, Guid courseId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Enrollment>> GetUserEnrollmentsAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<bool> IsEnrolledAsync(Guid userId, Guid courseId, CancellationToken cancellationToken = default);
    Task<int> GetEnrollmentCountByCourseAsync(Guid courseId, CancellationToken cancellationToken = default);
}

