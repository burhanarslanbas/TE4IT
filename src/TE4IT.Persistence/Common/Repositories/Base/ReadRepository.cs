using Microsoft.EntityFrameworkCore;
using TE4IT.Application.Abstractions.Persistence.Repositories.Base;
using TE4IT.Application.Common.Pagination;
using TE4IT.Domain.Entities.Common;
using TE4IT.Persistence.Common.Contexts;

namespace TE4IT.Persistence.Common.Repositories.Base;

public class ReadRepository<TAggregate> : IReadRepository<TAggregate> where TAggregate : AggregateRoot
{
    private readonly AppDbContext _db;
    public ReadRepository(AppDbContext db) => _db = db;
    protected DbSet<TAggregate> Table => _db.Set<TAggregate>();

    public Task<List<TAggregate>> GetAllAsync(CancellationToken cancellationToken = default)
        => Table.AsNoTracking().ToListAsync(cancellationToken);

    public Task<TAggregate?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => Table.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
        => Table.AsNoTracking().AnyAsync(x => x.Id == id, cancellationToken);

    public async Task<PagedResult<TAggregate>> ListAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = Table.AsNoTracking().OrderByDescending(x => x.CreatedDate);
        var total = await query.CountAsync(cancellationToken);
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
        return new PagedResult<TAggregate>(items, total, page, pageSize);
    }
}