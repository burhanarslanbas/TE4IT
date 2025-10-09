using TaskEntity = TE4IT.Domain.Entities.Task;

namespace TE4IT.Domain.Services;

/// <summary>
/// Görev bağımlılık servisi - görevler arası bağımlılık kurallarını yönetir
/// </summary>
public interface ITaskDependencyService
{
    /// <summary>
    /// Belirtilen görevin tamamlanabilir olup olmadığını kontrol eder
    /// </summary>
    /// <param name="task">Kontrol edilecek görev</param>
    /// <returns>Görev tamamlanabilirse true, aksi halde false</returns>
    bool CanCompleteTask(TaskEntity task);

    /// <summary>
    /// Görevin blokladığı diğer görevleri kontrol eder
    /// </summary>
    /// <param name="task">Kontrol edilecek görev</param>
    /// <returns>Blokladığı görevler varsa true, aksi halde false</returns>
    bool HasBlockingDependencies(TaskEntity task);

    /// <summary>
    /// İki görev arasında döngüsel bağımlılık olup olmadığını kontrol eder
    /// </summary>
    /// <param name="sourceTask">Kaynak görev</param>
    /// <param name="targetTask">Hedef görev</param>
    /// <returns>Döngüsel bağımlılık varsa true, aksi halde false</returns>
    bool HasCircularDependency(TaskEntity sourceTask, TaskEntity targetTask);

    /// <summary>
    /// Görevin bağımlılık derinliğini hesaplar
    /// </summary>
    /// <param name="task">Hesaplanacak görev</param>
    /// <returns>Bağımlılık derinliği</returns>
    int CalculateDependencyDepth(TaskEntity task);

    /// <summary>
    /// Görevin tüm bağımlılıklarını (önceki görevleri) getirir
    /// </summary>
    /// <param name="task">Görev</param>
    /// <returns>Bağımlılık zincirindeki tüm görevler</returns>
    IEnumerable<TaskEntity> GetDependencyChain(TaskEntity task);

    /// <summary>
    /// Görevin tamamlanmasını bekleyen görevleri getirir
    /// </summary>
    /// <param name="task">Görev</param>
    /// <returns>Bu görevi bekleyen görevler</returns>
    IEnumerable<TaskEntity> GetBlockedTasks(TaskEntity task);

    /// <summary>
    /// Görev bağımlılığı eklenebilir mi kontrol eder
    /// </summary>
    /// <param name="sourceTask">Kaynak görev</param>
    /// <param name="targetTask">Hedef görev</param>
    /// <param name="relationType">Bağımlılık türü</param>
    /// <returns>Bağımlılık eklenebilirse true, aksi halde false</returns>
    bool CanAddDependency(TaskEntity sourceTask, TaskEntity targetTask, Enums.TaskRelationType relationType);
}
