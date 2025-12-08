using TE4IT.Abstractions.Persistence.Repositories.ProjectMembers;
using TE4IT.Persistence.Common.Contexts;
using TE4IT.Persistence.Common.Repositories.Base;

namespace TE4IT.Persistence.TaskManagement.Repositories.ProjectMembers;

public sealed class ProjectMemberWriteRepository(AppDbContext db)
    : WriteRepository<Domain.Entities.ProjectMember>(db), IProjectMemberWriteRepository
{
}

