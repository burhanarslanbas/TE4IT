using Microsoft.EntityFrameworkCore;
using TE4IT.Abstractions.Persistence.Repositories.ProjectMembers;
using TE4IT.Domain.Entities;
using TE4IT.Domain.ValueObjects;
using TE4IT.Persistence.Common.Contexts;
using TE4IT.Persistence.Common.Repositories.Base;

namespace TE4IT.Persistence.TaskManagement.Repositories.ProjectMembers;

public sealed class ProjectMemberReadRepository(AppDbContext db)
    : ReadRepository<ProjectMember>(db), IProjectMemberReadRepository
{
    public async Task<List<ProjectMember>> GetByProjectIdAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        return await Table
            .Where(pm => pm.ProjectId == projectId)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<ProjectMember>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await Table
            .Where(pm => pm.UserId == (UserId)userId)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Guid>> GetProjectIdsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await Table
            .Where(pm => pm.UserId == (UserId)userId)
            .Select(pm => pm.ProjectId)
            .ToListAsync(cancellationToken);
    }

    public async Task<ProjectMember?> GetByProjectIdAndUserIdAsync(Guid projectId, Guid userId, CancellationToken cancellationToken = default)
    {
        return await Table
            .FirstOrDefaultAsync(pm => pm.ProjectId == projectId && pm.UserId == (UserId)userId, cancellationToken);
    }

    public Task<int> CountByProjectIdAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        return Table.CountAsync(pm => pm.ProjectId == projectId, cancellationToken);
    }
}

