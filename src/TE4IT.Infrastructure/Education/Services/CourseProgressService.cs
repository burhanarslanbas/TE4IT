using TE4IT.Application.Abstractions.Persistence.Repositories.Education.Courses;
using TE4IT.Application.Abstractions.Persistence.Repositories.Education.Progresses;
using TE4IT.Domain.Services;

namespace TE4IT.Infrastructure.Education.Services;

/// <summary>
/// Eğitim modülü ilerleme kurallarını hesaplayan domain servisi implementasyonu.
/// </summary>
public sealed class CourseProgressService(
    ICourseReadRepository courseReadRepository,
    IProgressReadRepository progressReadRepository) : ICourseProgressService
{
    public async Task<bool> CanAccessStepAsync(Guid userId, Guid stepId, CancellationToken cancellationToken = default)
    {
        // Step'i içeren course'u bul (tüm course'ları çekip filtrele)
        var allCourses = await courseReadRepository.GetAllAsync(cancellationToken);
        var course = allCourses.FirstOrDefault(c => 
            c.Roadmap?.Steps.Any(s => s.Id == stepId) == true);

        if (course is null || course.Roadmap is null)
        {
            return false;
        }

        // Step'i bul
        var step = course.Roadmap.Steps.FirstOrDefault(s => s.Id == stepId);
        if (step is null)
        {
            return false;
        }

        // İlk adımsa erişilebilir
        if (step.Order == 1)
        {
            return true;
        }

        // Önceki adımların tamamlanıp tamamlanmadığını kontrol et
        var previousSteps = course.Roadmap.Steps
            .Where(s => s.Order < step.Order && s.IsRequired)
            .OrderBy(s => s.Order)
            .ToList();

        foreach (var previousStep in previousSteps)
        {
            var isCompleted = await IsStepCompletedAsync(userId, previousStep.Id, cancellationToken);
            if (!isCompleted)
            {
                return false;
            }
        }

        return true;
    }

    public async Task<bool> IsStepCompletedAsync(Guid userId, Guid stepId, CancellationToken cancellationToken = default)
    {
        // Step'i içeren course'u bul (tüm course'ları çekip filtrele)
        var allCourses = await courseReadRepository.GetAllAsync(cancellationToken);
        var course = allCourses.FirstOrDefault(c => 
            c.Roadmap?.Steps.Any(s => s.Id == stepId) == true);

        if (course is null || course.Roadmap is null)
        {
            return false;
        }

        // Step'i bul
        var step = course.Roadmap.Steps.FirstOrDefault(s => s.Id == stepId);
        if (step is null)
        {
            return false;
        }

        // Step'teki zorunlu içerikleri kontrol et
        var requiredContents = step.Contents.Where(c => c.IsRequired).ToList();
        if (requiredContents.Count == 0)
        {
            return true; // Zorunlu içerik yoksa tamamlanmış sayılır
        }

        // Her zorunlu içeriğin tamamlanıp tamamlanmadığını kontrol et
        foreach (var content in requiredContents)
        {
            var progress = await progressReadRepository.GetByUserAndContentAsync(
                userId,
                content.Id,
                course.Id,
                cancellationToken);

            if (progress is null || !progress.IsCompleted)
            {
                return false;
            }
        }

        return true;
    }

    public async Task<bool> IsCourseCompletedAsync(Guid userId, Guid courseId, CancellationToken cancellationToken = default)
    {
        var course = await courseReadRepository.GetByIdAsync(courseId, cancellationToken);
        if (course is null || course.Roadmap is null)
        {
            return false;
        }

        // Tüm zorunlu adımların tamamlanıp tamamlanmadığını kontrol et
        var requiredSteps = course.Roadmap.Steps
            .Where(s => s.IsRequired)
            .OrderBy(s => s.Order)
            .ToList();

        foreach (var step in requiredSteps)
        {
            var isCompleted = await IsStepCompletedAsync(userId, step.Id, cancellationToken);
            if (!isCompleted)
            {
                return false;
            }
        }

        return true;
    }

    public async Task<int> CalculateProgressPercentageAsync(Guid userId, Guid courseId, CancellationToken cancellationToken = default)
    {
        var course = await courseReadRepository.GetByIdAsync(courseId, cancellationToken);
        if (course is null || course.Roadmap is null)
        {
            return 0;
        }

        // Tüm içerikleri say (zorunlu ve zorunlu olmayan)
        var allContents = course.Roadmap.Steps
            .SelectMany(s => s.Contents)
            .ToList();

        if (allContents.Count == 0)
        {
            return 0;
        }

        // Tamamlanan içerikleri say
        int completedCount = 0;
        foreach (var content in allContents)
        {
            var progress = await progressReadRepository.GetByUserAndContentAsync(
                userId,
                content.Id,
                courseId,
                cancellationToken);

            if (progress is not null && progress.IsCompleted)
            {
                completedCount++;
            }
        }

        // Yüzde hesapla
        return (int)Math.Round((decimal)completedCount / allContents.Count * 100);
    }

    public async Task<Guid?> GetNextUnlockedStepIdAsync(Guid userId, Guid courseId, CancellationToken cancellationToken = default)
    {
        var course = await courseReadRepository.GetByIdAsync(courseId, cancellationToken);
        if (course is null || course.Roadmap is null)
        {
            return null;
        }

        // Sırayla adımları kontrol et
        var orderedSteps = course.Roadmap.Steps
            .OrderBy(s => s.Order)
            .ToList();

        foreach (var step in orderedSteps)
        {
            var canAccess = await CanAccessStepAsync(userId, step.Id, cancellationToken);
            if (canAccess)
            {
                var isCompleted = await IsStepCompletedAsync(userId, step.Id, cancellationToken);
                if (!isCompleted)
                {
                    return step.Id;
                }
            }
        }

        return null; // Tüm adımlar tamamlanmış
    }
}

