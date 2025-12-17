using System.Threading.Tasks;
using TE4IT.Domain.Entities;

namespace TE4IT.Application.Abstractions.Persistence.Repositories.TaskRelations;

public interface ITaskRelationWriteRepository
{
    System.Threading.Tasks.Task AddAsync(TaskRelation taskRelation, CancellationToken cancellationToken = default);
}

