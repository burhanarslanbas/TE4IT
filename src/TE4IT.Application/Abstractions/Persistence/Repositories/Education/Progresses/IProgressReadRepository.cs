using TE4IT.Application.Abstractions.Persistence.Repositories.Base;
using TE4IT.Domain.Entities.Education;

namespace TE4IT.Application.Abstractions.Persistence.Repositories.Education.Progresses;

public interface IProgressReadRepository : IReadRepository<Progress>
{
    Task<Progress?> GetByUserAndContentAsync(Guid userId, Guid contentId, Guid courseId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Progress>> GetUserProgressByCourseAsync(Guid userId, Guid courseId, CancellationToken cancellationToken = default);
    Task<int> GetCompletedContentCountAsync(Guid userId, Guid stepId, Guid courseId, CancellationToken cancellationToken = default);
}

