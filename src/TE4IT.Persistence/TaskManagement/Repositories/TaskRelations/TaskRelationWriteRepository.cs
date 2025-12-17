using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TE4IT.Application.Abstractions.Persistence.Repositories.TaskRelations;
using TE4IT.Domain.Entities;
using TE4IT.Persistence.Common.Contexts;

namespace TE4IT.Persistence.TaskManagement.Repositories.TaskRelations;

public sealed class TaskRelationWriteRepository(AppDbContext db) : ITaskRelationWriteRepository
{
    public System.Threading.Tasks.Task AddAsync(TaskRelation taskRelation, CancellationToken cancellationToken = default)
    {
        return db.TaskRelations.AddAsync(taskRelation, cancellationToken).AsTask();
    }
}

