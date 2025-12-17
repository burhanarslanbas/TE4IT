using Microsoft.EntityFrameworkCore;
using TE4IT.Abstractions.Persistence.Repositories.UseCases;
using TE4IT.Domain.Entities;
using TE4IT.Persistence.Common.Contexts;
using TE4IT.Persistence.Common.Repositories.Base;

namespace TE4IT.Persistence.TaskManagement.Repositories.UseCases;

public sealed class UseCaseWriteRepository(AppDbContext db)
    : WriteRepository<UseCase>(db), IUseCaseWriteRepository
{
    public async System.Threading.Tasks.Task ArchiveByModuleIdAsync(Guid moduleId, CancellationToken cancellationToken = default)
    {
        var useCases = await Table
            .Where(uc => uc.ModuleId == moduleId && uc.IsActive)
            .ToListAsync(cancellationToken);
        
        foreach (var useCase in useCases)
        {
            useCase.Archive();
        }
    }
}

