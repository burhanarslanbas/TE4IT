using TE4IT.Domain.Services;
using TaskEntity = TE4IT.Domain.Entities.Task;

namespace TE4IT.Domain.Specifications;

/// <summary>
/// Görevin geçerli bağımlılıkları olup olmadığını kontrol eden specification
/// </summary>
public class TaskHasValidDependenciesSpecification
{
    private readonly ITaskDependencyService _dependencyService;

    public TaskHasValidDependenciesSpecification(ITaskDependencyService dependencyService)
    {
        _dependencyService = dependencyService;
    }
    /// <summary>
    /// Görevin geçerli bağımlılıkları olup olmadığını kontrol eder
    /// </summary>
    /// <param name="task">Kontrol edilecek görev</param>
    /// <returns>Geçerli bağımlılıklar varsa true, aksi halde false</returns>
    public bool IsSatisfiedBy(TaskEntity task)
    {
        if (task == null)
            return false;

        // Service aracılığıyla kurallar
        if (_dependencyService.HasCircularDependency(task, task)) // sembolik, gerçek kullanım target ister
            return false;
        if (_dependencyService.CalculateDependencyDepth(task) > Constants.DomainConstants.MaxTaskDependencyDepth)
            return false;

        return true;
    }

    /// <summary>
    /// Görevin geçersiz bağımlılık sebebini getirir
    /// </summary>
    /// <param name="task">Kontrol edilecek görev</param>
    /// <returns>Geçersizlik sebebi</returns>
    public string GetReason(TaskEntity task)
    {
        if (task == null)
            return "Görev bulunamadı.";

        if (_dependencyService.HasCircularDependency(task, task))
            return "Döngüsel bağımlılık tespit edildi.";
        if (_dependencyService.CalculateDependencyDepth(task) > Constants.DomainConstants.MaxTaskDependencyDepth)
            return "Maksimum bağımlılık derinliği aşıldı.";

        return "Bağımlılıklar geçerli.";
    }
}
