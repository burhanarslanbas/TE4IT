using TE4IT.Application.Abstractions.Persistence.Repositories.Base;
using TE4IT.Application.Common.Pagination;

namespace TE4IT.Abstractions.Persistence.Repositories.UseCases;

public interface IUseCaseReadRepository : IReadRepository<Domain.Entities.UseCase>
{
    Task<int> CountByModuleAsync(Guid moduleId, CancellationToken cancellationToken = default);
    Task<Dictionary<Guid, int>> CountByModuleIdsAsync(List<Guid> moduleIds, CancellationToken cancellationToken = default);
    Task<PagedResult<Domain.Entities.UseCase>> GetByModuleIdAsync(
        Guid moduleId,
        int page,
        int pageSize,
        bool? isActive = null,
        string? search = null,
        CancellationToken cancellationToken = default);
}

