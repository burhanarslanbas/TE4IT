using TE4IT.Application.Abstractions.Persistence.Repositories.Base;
using TE4IT.Domain.Entities.Education;

namespace TE4IT.Application.Abstractions.Persistence.Repositories.Education.Progresses;

public interface IProgressWriteRepository : IWriteRepository<Progress>
{
    Task UpsertAsync(Progress progress, CancellationToken cancellationToken = default);
}

