using TE4IT.Application.Abstractions.Persistence.Repositories.Base;
using TE4IT.Application.Common.Pagination;

namespace TE4IT.Abstractions.Persistence.Repositories.Projects;

public interface IProjectReadRepository : IReadRepository<Domain.Entities.Project>
{
    Task<int> CountByCreatorAsync(Guid creatorId, CancellationToken cancellationToken = default);
    Task<PagedResult<Domain.Entities.Project>> GetByUserAccessAsync(
        Guid userId,
        bool isAdmin,
        int page,
        int pageSize,
        bool? isActive = null,
        string? search = null,
        CancellationToken cancellationToken = default);
}