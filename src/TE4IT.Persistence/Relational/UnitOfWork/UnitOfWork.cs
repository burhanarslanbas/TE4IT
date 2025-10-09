using TE4IT.Application.Abstractions.Persistence;
using TE4IT.Persistence.Relational.Db;

namespace TE4IT.Persistence.Relational;

public sealed class UnitOfWork(AppDbContext db) : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => db.SaveChangesAsync(cancellationToken);
}


