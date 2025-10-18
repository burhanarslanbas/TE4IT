using TE4IT.Abstractions.Persistence.Repositories.Tasks;
using TE4IT.Persistence.Common.Contexts;
using TE4IT.Persistence.Common.Repositories.Base;

namespace TE4IT.Persistence.TaskManagement.Repositories.Tasks;

public sealed class TaskReadRepository(AppDbContext db)
    : ReadRepository<Domain.Entities.Task>(db), ITaskReadRepository
{
}

