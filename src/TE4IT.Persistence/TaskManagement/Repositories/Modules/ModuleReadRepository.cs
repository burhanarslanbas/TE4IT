using Microsoft.EntityFrameworkCore;
using TE4IT.Abstractions.Persistence.Repositories.Modules;
using TE4IT.Application.Common.Pagination;
using TE4IT.Domain.Entities;
using TE4IT.Persistence.Common.Contexts;
using TE4IT.Persistence.Common.Repositories.Base;

namespace TE4IT.Persistence.TaskManagement.Repositories.Modules;

public sealed class ModuleReadRepository(AppDbContext db)
    : ReadRepository<Module>(db), IModuleReadRepository
{
    public Task<int> CountByProjectAsync(Guid projectId, CancellationToken cancellationToken = default)
        => Table.CountAsync(m => m.ProjectId == projectId, cancellationToken);

    public async Task<PagedResult<Module>> GetByProjectIdAsync(
        Guid projectId,
        int page,
        int pageSize,
        bool? isActive = null,
        string? search = null,
        CancellationToken cancellationToken = default)
    {
        var query = Table.Where(m => m.ProjectId == projectId);

        if (isActive.HasValue)
            query = query.Where(m => m.IsActive == isActive.Value);

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(m => m.Title.Contains(search) || (m.Description != null && m.Description.Contains(search)));

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Module>(items, totalCount, page, pageSize);
    }
}

