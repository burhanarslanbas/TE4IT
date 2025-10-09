using TE4IT.Domain.Entities;

namespace TE4IT.Domain.Services;

/// <summary>
/// Proje ilerleme servisi - proje ve modül ilerlemelerini hesaplar
/// </summary>
public interface IProjectProgressService
{
    /// <summary>
    /// Projenin genel ilerleme yüzdesini hesaplar
    /// </summary>
    /// <param name="project">Proje</param>
    /// <returns>İlerleme yüzdesi (0-100)</returns>
    decimal CalculateProjectProgress(Project project);

    /// <summary>
    /// Modülün ilerleme yüzdesini hesaplar
    /// </summary>
    /// <param name="module">Modül</param>
    /// <returns>İlerleme yüzdesi (0-100)</returns>
    decimal CalculateModuleProgress(Module module);

    /// <summary>
    /// Use case'in ilerleme yüzdesini hesaplar
    /// </summary>
    /// <param name="useCase">Use case</param>
    /// <returns>İlerleme yüzdesi (0-100)</returns>
    decimal CalculateUseCaseProgress(UseCase useCase);

    /// <summary>
    /// Projenin tahmini tamamlanma tarihini hesaplar
    /// </summary>
    /// <param name="project">Proje</param>
    /// <returns>Tahmini tamamlanma tarihi</returns>
    DateTime? CalculateEstimatedCompletionDate(Project project);

    /// <summary>
    /// Projenin gecikme durumunu kontrol eder
    /// </summary>
    /// <param name="project">Proje</param>
    /// <returns>Gecikme durumu bilgisi</returns>
    ProjectDelayStatus GetProjectDelayStatus(Project project);

    /// <summary>
    /// Proje istatistiklerini getirir
    /// </summary>
    /// <param name="project">Proje</param>
    /// <returns>Proje istatistikleri</returns>
    ProjectStatistics GetProjectStatistics(Project project);
}

/// <summary>
/// Proje gecikme durumu
/// </summary>
public record ProjectDelayStatus(
    bool IsDelayed,
    int DaysDelayed,
    string DelayReason,
    DateTime? OriginalDeadline,
    DateTime? EstimatedCompletion
);

/// <summary>
/// Proje istatistikleri
/// </summary>
public record ProjectStatistics(
    int TotalTasks,
    int CompletedTasks,
    int InProgressTasks,
    int NotStartedTasks,
    int CancelledTasks,
    int TotalModules,
    int CompletedModules,
    int TotalUseCases,
    int CompletedUseCases,
    decimal OverallProgress,
    TimeSpan EstimatedRemainingTime,
    DateTime? EstimatedCompletionDate
);
