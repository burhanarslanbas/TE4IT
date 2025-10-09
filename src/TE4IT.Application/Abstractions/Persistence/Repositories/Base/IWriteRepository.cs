using TE4IT.Domain.Entities.Common;

namespace TE4IT.Application.Abstractions.Persistence.Repositories.Base;

public interface IWriteRepository<TAggregate> where TAggregate : AggregateRoot
{
    Task AddAsync(TAggregate entity, CancellationToken cancellationToken = default);
    void Update(TAggregate entity, CancellationToken cancellationToken = default);
    void Remove(TAggregate entity, CancellationToken cancellationToken = default);
}
