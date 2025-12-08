using TE4IT.Application.Abstractions.Persistence.Repositories.Base;
using TE4IT.Application.Common.Pagination;

namespace TE4IT.Abstractions.Persistence.Repositories.Modules;

public interface IModuleReadRepository : IReadRepository<Domain.Entities.Module>
{
    Task<int> CountByProjectAsync(Guid projectId, CancellationToken cancellationToken = default);
    Task<PagedResult<Domain.Entities.Module>> GetByProjectIdAsync(
        Guid projectId,
        int page,
        int pageSize,
        bool? isActive = null,
        string? search = null,
        CancellationToken cancellationToken = default);
}

