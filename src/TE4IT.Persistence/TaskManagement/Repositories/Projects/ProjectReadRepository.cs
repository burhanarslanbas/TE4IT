using Microsoft.EntityFrameworkCore;
using TE4IT.Abstractions.Persistence.Repositories.Projects;
using TE4IT.Application.Common.Pagination;
using TE4IT.Domain.Entities;
using TE4IT.Domain.ValueObjects;
using TE4IT.Persistence.Common.Contexts;
using TE4IT.Persistence.Common.Repositories.Base;

namespace TE4IT.Persistence.TaskManagement.Repositories.Projects;

public sealed class ProjectReadRepository(AppDbContext db) : ReadRepository<Project>(db), IProjectReadRepository
{
    public Task<int> CountByCreatorAsync(Guid creatorId, CancellationToken cancellationToken = default)
        => Table.CountAsync(p => p.CreatorId == (UserId)creatorId, cancellationToken);

    public async Task<PagedResult<Project>> GetByUserAccessAsync(
        Guid userId,
        bool isAdmin,
        int page,
        int pageSize,
        bool? isActive = null,
        string? search = null,
        CancellationToken cancellationToken = default)
    {
        var query = Table.AsQueryable();

        // Admin ise tüm projeleri göster
        if (!isAdmin)
        {
            var userIdValue = (UserId)userId;
            
            // Kullanıcının erişebileceği projeleri tek query ile filtrele:
            // 1. Kullanıcının üye olduğu projeler (ProjectMember)
            // 2. Kullanıcının oluşturduğu projeler (CreatorId)
            query = query.Where(p => 
                p.CreatorId == userIdValue || 
                db.Set<Domain.Entities.ProjectMember>()
                    .Any(pm => pm.ProjectId == p.Id && pm.UserId == userIdValue));
        }

        // Filtreleme
        if (isActive.HasValue)
            query = query.Where(p => p.IsActive == isActive.Value);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchLower = search.ToLower();
            query = query.Where(p => 
                p.Title.ToLower().Contains(searchLower) || 
                (p.Description != null && p.Description.ToLower().Contains(searchLower)));
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(p => p.CreatedDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Project>(items, totalCount, page, pageSize);
    }
}
