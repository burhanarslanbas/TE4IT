using TE4IT.Application.Common.Pagination;
using TE4IT.Domain.Entities.Common;

namespace TE4IT.Application.Abstractions.Persistence.Repositories.Base;

public interface IReadRepository<TAggregate> where TAggregate : AggregateRoot
{
    Task<List<TAggregate>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<TAggregate?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PagedResult<TAggregate>> ListAsync(int page, int pageSize, CancellationToken cancellationToken = default);
}
