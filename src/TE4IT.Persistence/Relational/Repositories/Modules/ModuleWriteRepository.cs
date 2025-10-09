using TE4IT.Abstractions.Persistence.Repositories.Modules;
using TE4IT.Domain.Entities;
using TE4IT.Persistence.Relational.Db;
using TE4IT.Persistence.Relational.Repositories.Base;

namespace TE4IT.Persistence.Relational.Repositories.Modules;

public sealed class ModuleWriteRepository(AppDbContext db)
    : WriteRepository<Module>(db), IModuleWriteRepository
{
}

