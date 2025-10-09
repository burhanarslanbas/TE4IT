using TE4IT.Domain.Enums;
using TaskEntity = TE4IT.Domain.Entities.Task;

namespace TE4IT.Domain.Specifications;

/// <summary>
/// Görevin tamamlanabilir olup olmadığını kontrol eden specification
/// </summary>
public class TaskCanBeCompletedSpecification
{
    /// <summary>
    /// Görevin tamamlanabilir olup olmadığını kontrol eder
    /// </summary>
    /// <param name="task">Kontrol edilecek görev</param>
    /// <returns>Görev tamamlanabilirse true, aksi halde false</returns>
    public bool IsSatisfiedBy(TaskEntity task)
    {
        if (task == null)
            return false;

        // Görev zaten tamamlanmışsa
        if (task.TaskState == TaskState.Completed)
            return false;

        // Görev iptal edilmişse
        if (task.TaskState == TaskState.Cancelled)
            return false;

        // Görev başlatılmamışsa tamamlanamaz
        if (task.TaskState == TaskState.NotStarted)
            return false;

        // Görev devam ediyorsa tamamlanabilir
        if (task.TaskState == TaskState.InProgress)
            return true;

        return false;
    }

    /// <summary>
    /// Görevin tamamlanabilir olmama sebebini getirir
    /// </summary>
    /// <param name="task">Kontrol edilecek görev</param>
    /// <returns>Tamamlanamama sebebi</returns>
    public string GetReason(TaskEntity task)
    {
        if (task == null)
            return "Görev bulunamadı.";

        return task.TaskState switch
        {
            TaskState.Completed => "Görev zaten tamamlanmış.",
            TaskState.Cancelled => "İptal edilmiş görev tamamlanamaz.",
            TaskState.NotStarted => "Başlatılmamış görev tamamlanamaz.",
            TaskState.InProgress => "Görev tamamlanabilir.",
            _ => "Bilinmeyen görev durumu."
        };
    }
}
