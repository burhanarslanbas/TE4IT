using TE4IT.Application.Abstractions.Persistence.Repositories.ProjectInvitations;
using TE4IT.Domain.Entities;
using TE4IT.Persistence.Common.Contexts;
using TE4IT.Persistence.Common.Repositories.Base;

namespace TE4IT.Persistence.TaskManagement.Repositories.ProjectInvitations;

public sealed class ProjectInvitationWriteRepository(AppDbContext db)
    : WriteRepository<ProjectInvitation>(db), IProjectInvitationWriteRepository
{
}

