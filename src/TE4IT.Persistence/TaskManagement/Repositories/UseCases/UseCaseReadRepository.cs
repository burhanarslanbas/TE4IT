using Microsoft.EntityFrameworkCore;
using TE4IT.Abstractions.Persistence.Repositories.UseCases;
using TE4IT.Application.Common.Pagination;
using TE4IT.Domain.Entities;
using TE4IT.Persistence.Common.Contexts;
using TE4IT.Persistence.Common.Repositories.Base;

namespace TE4IT.Persistence.TaskManagement.Repositories.UseCases;

public sealed class UseCaseReadRepository(AppDbContext db)
    : ReadRepository<UseCase>(db), IUseCaseReadRepository
{
    public Task<int> CountByModuleAsync(Guid moduleId, CancellationToken cancellationToken = default)
        => Table.CountAsync(uc => uc.ModuleId == moduleId, cancellationToken);

    public async Task<Dictionary<Guid, int>> CountByModuleIdsAsync(List<Guid> moduleIds, CancellationToken cancellationToken = default)
    {
        return await Table
            .Where(uc => moduleIds.Contains(uc.ModuleId))
            .GroupBy(uc => uc.ModuleId)
            .Select(g => new { ModuleId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.ModuleId, x => x.Count, cancellationToken);
    }

    public async Task<PagedResult<UseCase>> GetByModuleIdAsync(
        Guid moduleId,
        int page,
        int pageSize,
        bool? isActive = null,
        string? search = null,
        CancellationToken cancellationToken = default)
    {
        var query = Table.Where(uc => uc.ModuleId == moduleId);

        if (isActive.HasValue)
            query = query.Where(uc => uc.IsActive == isActive.Value);

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(uc => uc.Title.Contains(search) || (uc.Description != null && uc.Description.Contains(search)));

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<UseCase>(items, totalCount, page, pageSize);
    }
}

