using TE4IT.Abstractions.Persistence.Repositories.Projects;
using TE4IT.Domain.Entities;
using TE4IT.Persistence.Common.Contexts;
using TE4IT.Persistence.Common.Repositories.Base;

namespace TE4IT.Persistence.TaskManagement.Repositories.Projects;

public sealed class ProjectReadRepository(AppDbContext db)
    : ReadRepository<Project>(db), IProjectReadRepository
{
}
