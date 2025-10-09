using Microsoft.EntityFrameworkCore;
using TE4IT.Application.Abstractions.Persistence.Repositories.Base;
using TE4IT.Domain.Entities.Common;
using TE4IT.Persistence.Relational.Db;

namespace TE4IT.Persistence.Relational.Repositories.Base;

public class WriteRepository<TAggregate> : IWriteRepository<TAggregate> where TAggregate : AggregateRoot
{
    private readonly AppDbContext _db;
    public WriteRepository(AppDbContext db) => _db = db;
    protected DbSet<TAggregate> Table => _db.Set<TAggregate>();

    public Task AddAsync(TAggregate entity, CancellationToken cancellationToken = default) => Table.AddAsync(entity, cancellationToken).AsTask();

    public void Update(TAggregate entity, CancellationToken cancellationToken = default) => Table.Update(entity);

    public void Remove(TAggregate entity, CancellationToken cancellationToken = default) => Table.Remove(entity);
}

