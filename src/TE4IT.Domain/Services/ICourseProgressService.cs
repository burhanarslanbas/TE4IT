namespace TE4IT.Domain.Services;

/// <summary>
/// Eğitim modülü ilerleme kurallarını hesaplayan domain servisi.
/// </summary>
public interface ICourseProgressService
{
    Task<bool> CanAccessStepAsync(Guid userId, Guid stepId, CancellationToken cancellationToken = default);
    Task<bool> IsStepCompletedAsync(Guid userId, Guid stepId, CancellationToken cancellationToken = default);
    Task<bool> IsCourseCompletedAsync(Guid userId, Guid courseId, CancellationToken cancellationToken = default);
    Task<int> CalculateProgressPercentageAsync(Guid userId, Guid courseId, CancellationToken cancellationToken = default);
    Task<Guid?> GetNextUnlockedStepIdAsync(Guid userId, Guid courseId, CancellationToken cancellationToken = default);
}

