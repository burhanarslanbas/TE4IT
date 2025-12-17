using Microsoft.EntityFrameworkCore;
using TE4IT.Application.Abstractions.Persistence.Repositories.ProjectInvitations;
using TE4IT.Domain.Entities;
using TE4IT.Persistence.Common.Contexts;
using TE4IT.Persistence.Common.Repositories.Base;

namespace TE4IT.Persistence.TaskManagement.Repositories.ProjectInvitations;

public sealed class ProjectInvitationReadRepository(AppDbContext db)
    : ReadRepository<ProjectInvitation>(db), IProjectInvitationReadRepository
{
    public async Task<ProjectInvitation?> GetByTokenHashAsync(string tokenHash, CancellationToken cancellationToken = default)
    {
        return await Table
            .FirstOrDefaultAsync(pi => pi.TokenHash == tokenHash, cancellationToken);
    }

    public async Task<List<ProjectInvitation>> GetByProjectIdAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        return await Table
            .Where(pi => pi.ProjectId == projectId)
            .OrderByDescending(pi => pi.CreatedDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<ProjectInvitation>> GetPendingByProjectIdAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        return await Table
            .Where(pi => pi.ProjectId == projectId
                && pi.AcceptedAt == null
                && pi.CancelledAt == null
                && pi.ExpiresAt > now)
            .OrderByDescending(pi => pi.CreatedDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<ProjectInvitation?> GetByProjectIdAndEmailAsync(Guid projectId, string email, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = email.ToLowerInvariant().Trim();
        return await Table
            .FirstOrDefaultAsync(pi => pi.ProjectId == projectId && pi.Email == normalizedEmail, cancellationToken);
    }

    public async Task<List<ProjectInvitation>> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = email.ToLowerInvariant().Trim();
        return await Table
            .Where(pi => pi.Email == normalizedEmail)
            .OrderByDescending(pi => pi.CreatedDate)
            .ToListAsync(cancellationToken);
    }
}

