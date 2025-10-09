using TE4IT.Domain.Entities;

namespace TE4IT.Domain.Specifications;

/// <summary>
/// Projenin arşivlenebilir olup olmadığını kontrol eden specification
/// Aggregate sınırlarına saygı duyar; cross-aggregate durumları burada dolaşmaz
/// </summary>
public class ProjectCanBeArchivedSpecification
{
    /// <summary>
    /// Projenin arşivlenebilir olup olmadığını kontrol eder
    /// </summary>
    /// <param name="project">Kontrol edilecek proje</param>
    /// <returns>Arşivlenebilirse true, aksi halde false</returns>
    public bool IsSatisfiedBy(Project project)
    {
        if (project == null)
            return false;

        // Proje zaten arşivlenmişse
        if (!project.IsActive)
            return false;

        // Cross-aggregate kontroller Application katmanında yapılmalı (repo/sorgular ile)
        // Burada yalnızca Project aggregate'ın kendi durumuna göre karar veriyoruz
        return true;
    }

    /// <summary>
    /// Projenin arşivlenememe sebebini getirir
    /// </summary>
    /// <param name="project">Kontrol edilecek proje</param>
    /// <returns>Arşivlenememe sebebi</returns>
    public string GetReason(Project project)
    {
        if (project == null)
            return "Proje bulunamadı.";

        if (!project.IsActive)
            return "Proje zaten arşivlenmiş.";

        // Diğer nedenler aggregate dışı veriye ihtiyaç duyduğundan burada değerlendirilmez
        return "Proje arşivlenebilir.";
    }
}
