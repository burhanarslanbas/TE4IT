using TE4IT.Abstractions.Persistence.Repositories.Tasks;
using TE4IT.Persistence.Relational.Db;
using TE4IT.Persistence.Relational.Repositories.Base;

namespace TE4IT.Persistence.Relational.Repositories.Tasks;

public sealed class TaskReadRepository(AppDbContext db)
    : ReadRepository<Domain.Entities.Task>(db), ITaskReadRepository
{
}

