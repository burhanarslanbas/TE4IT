using System.Threading.Tasks;
using TE4IT.Application.Abstractions.Persistence.Repositories.Base;
using TaskEntity = TE4IT.Domain.Entities.Task;
using TE4IT.Domain.Enums;
using TE4IT.Domain.ValueObjects;

namespace TE4IT.Abstractions.Persistence.Repositories.Tasks;

public interface ITaskReadRepository : IReadRepository<TaskEntity>
{
    Task<int> CountByUseCaseAsync(Guid useCaseId, CancellationToken cancellationToken = default);
    Task<List<TaskEntity>> GetByUseCaseIdAsync(
        Guid useCaseId,
        TaskState? state = null,
        TaskType? type = null,
        Guid? assigneeId = null,
        DateTime? dueDateFrom = null,
        DateTime? dueDateTo = null,
        string? search = null,
        CancellationToken cancellationToken = default);
    Task<TaskEntity?> GetByIdWithRelationsAsync(Guid taskId, CancellationToken cancellationToken = default);
}

