using TE4IT.Abstractions.Persistence.Repositories.Modules;
using TE4IT.Domain.Entities;
using TE4IT.Persistence.Common.Contexts;
using TE4IT.Persistence.Common.Repositories.Base;

namespace TE4IT.Persistence.TaskManagement.Repositories.Modules;

public sealed class ModuleReadRepository(AppDbContext db)
    : ReadRepository<Module>(db), IModuleReadRepository
{
}

