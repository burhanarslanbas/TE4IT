using TE4IT.Application.Abstractions.Persistence.Repositories.Base;

namespace TE4IT.Abstractions.Persistence.Repositories.Projects;

public interface IProjectReadRepository : IReadRepository<Domain.Entities.Project>
{
    Task<int> CountByCreatorAsync(Guid creatorId, CancellationToken cancellationToken = default);
}