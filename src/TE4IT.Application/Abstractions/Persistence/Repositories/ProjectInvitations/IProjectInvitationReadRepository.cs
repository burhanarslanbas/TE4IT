using TE4IT.Application.Abstractions.Persistence.Repositories.Base;
using TE4IT.Domain.Entities;

namespace TE4IT.Application.Abstractions.Persistence.Repositories.ProjectInvitations;

public interface IProjectInvitationReadRepository : IReadRepository<ProjectInvitation>
{
    /// <summary>
    /// Token hash'i ile daveti bulur
    /// </summary>
    Task<ProjectInvitation?> GetByTokenHashAsync(string tokenHash, CancellationToken cancellationToken = default);

    /// <summary>
    /// Proje ID'sine göre tüm davetleri getirir
    /// </summary>
    Task<List<ProjectInvitation>> GetByProjectIdAsync(Guid projectId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Proje ID'sine göre bekleyen (pending) davetleri getirir
    /// </summary>
    Task<List<ProjectInvitation>> GetPendingByProjectIdAsync(Guid projectId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Proje ID ve email'e göre daveti bulur
    /// </summary>
    Task<ProjectInvitation?> GetByProjectIdAndEmailAsync(Guid projectId, string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Email'e göre tüm davetleri getirir
    /// </summary>
    Task<List<ProjectInvitation>> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
}

