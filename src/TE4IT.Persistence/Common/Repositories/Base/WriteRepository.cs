using Microsoft.EntityFrameworkCore;
using TE4IT.Application.Abstractions.Persistence.Repositories.Base;
using TE4IT.Domain.Entities.Common;
using TE4IT.Persistence.Common.Contexts;

namespace TE4IT.Persistence.Common.Repositories.Base;

public class WriteRepository<TAggregate> : IWriteRepository<TAggregate> where TAggregate : AggregateRoot
{
    private readonly AppDbContext _db;
    public WriteRepository(AppDbContext db) => _db = db;
    protected DbSet<TAggregate> Table => _db.Set<TAggregate>();

    public Task AddAsync(TAggregate entity, CancellationToken cancellationToken = default) => Table.AddAsync(entity, cancellationToken).AsTask();

    public void Update(TAggregate entity, CancellationToken cancellationToken = default)
    {
        var entry = _db.Entry(entity);
        if (entry.State == EntityState.Detached)
        {
            Table.Update(entity);
        }
        else
        {
            // Entity zaten track edilmiş, sadece state'i Modified olarak işaretle
            entry.State = EntityState.Modified;
        }
    }

    public void Remove(TAggregate entity, CancellationToken cancellationToken = default) => Table.Remove(entity);
}

