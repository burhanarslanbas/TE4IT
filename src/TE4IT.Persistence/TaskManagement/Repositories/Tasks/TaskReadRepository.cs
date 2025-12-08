using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TE4IT.Abstractions.Persistence.Repositories.Tasks;
using TaskEntity = TE4IT.Domain.Entities.Task;
using TE4IT.Domain.Enums;
using TE4IT.Domain.ValueObjects;
using TE4IT.Persistence.Common.Contexts;
using TE4IT.Persistence.Common.Repositories.Base;

namespace TE4IT.Persistence.TaskManagement.Repositories.Tasks;

public sealed class TaskReadRepository(AppDbContext db)
    : ReadRepository<TaskEntity>(db), ITaskReadRepository
{
    public Task<int> CountByUseCaseAsync(Guid useCaseId, CancellationToken cancellationToken = default)
        => Table.CountAsync(t => t.UseCaseId == useCaseId, cancellationToken);

    public async Task<List<TaskEntity>> GetByUseCaseIdAsync(
        Guid useCaseId,
        TaskState? state = null,
        TaskType? type = null,
        Guid? assigneeId = null,
        DateTime? dueDateFrom = null,
        DateTime? dueDateTo = null,
        string? search = null,
        CancellationToken cancellationToken = default)
    {
        var query = Table.Where(t => t.UseCaseId == useCaseId);

        if (state.HasValue)
            query = query.Where(t => t.TaskState == state.Value);

        if (type.HasValue)
            query = query.Where(t => t.TaskType == type.Value);

        if (assigneeId.HasValue)
            query = query.Where(t => t.AssigneeId == (UserId)assigneeId.Value);

        if (dueDateFrom.HasValue)
            query = query.Where(t => t.DueDate.HasValue && t.DueDate.Value >= dueDateFrom.Value);

        if (dueDateTo.HasValue)
            query = query.Where(t => t.DueDate.HasValue && t.DueDate.Value <= dueDateTo.Value);

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(t => t.Title.Contains(search));

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<TaskEntity?> GetByIdWithRelationsAsync(Guid taskId, CancellationToken cancellationToken = default)
    {
        return await Table
            .Include(t => t.Relations)
            .FirstOrDefaultAsync(t => t.Id == taskId, cancellationToken);
    }
}

