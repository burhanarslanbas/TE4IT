using TE4IT.Abstractions.Persistence.Repositories.Projects;
using TE4IT.Domain.Entities;
using TE4IT.Persistence.Relational.Db;
using TE4IT.Persistence.Relational.Repositories.Base;

namespace TE4IT.Persistence.Relational.Repositories.Projects;

public sealed class ProjectWriteRepository(AppDbContext db)
    : WriteRepository<Project>(db), IProjectWriteRepository
{
}