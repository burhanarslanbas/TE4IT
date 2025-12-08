using Microsoft.EntityFrameworkCore;
using TE4IT.Abstractions.Persistence.Repositories.Projects;
using TE4IT.Application.Common.Pagination;
using TE4IT.Domain.Entities;
using TE4IT.Domain.ValueObjects;
using TE4IT.Persistence.Common.Contexts;
using TE4IT.Persistence.Common.Repositories.Base;

namespace TE4IT.Persistence.TaskManagement.Repositories.Projects;

public sealed class ProjectReadRepository(AppDbContext db)
    : ReadRepository<Project>(db), IProjectReadRepository
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
            // Kullanıcının sahip olduğu veya üye olduğu projeleri filtrele
            var userProjectIds = await db.Set<Domain.Entities.ProjectMember>()
                .Where(pm => pm.UserId == (UserId)userId)
                .Select(pm => pm.ProjectId)
                .ToListAsync(cancellationToken);

            // Ayrıca kullanıcının oluşturduğu projeleri de ekle (CreatorId kontrolü)
            var creatorProjectIds = await Table
                .Where(p => p.CreatorId == (UserId)userId)
                .Select(p => p.Id)
                .ToListAsync(cancellationToken);

            var allAccessibleProjectIds = userProjectIds.Union(creatorProjectIds).ToList();

            if (allAccessibleProjectIds.Count == 0)
            {
                // Kullanıcının erişebileceği proje yoksa boş liste döndür
                return new PagedResult<Project>(new List<Project>(), 0, page, pageSize);
            }

            query = query.Where(p => allAccessibleProjectIds.Contains(p.Id));
        }

        // Filtreleme
        if (isActive.HasValue)
            query = query.Where(p => p.IsActive == isActive.Value);

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(p => p.Title.Contains(search) || (p.Description != null && p.Description.Contains(search)));

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(p => p.CreatedDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Project>(items, totalCount, page, pageSize);
    }
}
