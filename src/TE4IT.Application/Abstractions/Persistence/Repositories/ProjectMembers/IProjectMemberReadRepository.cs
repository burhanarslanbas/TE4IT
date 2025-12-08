using TE4IT.Application.Abstractions.Persistence.Repositories.Base;

namespace TE4IT.Abstractions.Persistence.Repositories.ProjectMembers;

public interface IProjectMemberReadRepository : IReadRepository<Domain.Entities.ProjectMember>
{
    Task<List<Domain.Entities.ProjectMember>> GetByProjectIdAsync(Guid projectId, CancellationToken cancellationToken = default);
    Task<List<Domain.Entities.ProjectMember>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<List<Guid>> GetProjectIdsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Domain.Entities.ProjectMember?> GetByProjectIdAndUserIdAsync(Guid projectId, Guid userId, CancellationToken cancellationToken = default);
    Task<int> CountByProjectIdAsync(Guid projectId, CancellationToken cancellationToken = default);
}

