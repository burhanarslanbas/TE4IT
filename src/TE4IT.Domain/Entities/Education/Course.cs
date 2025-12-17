using TE4IT.Domain.Entities.Common;

namespace TE4IT.Domain.Entities.Education;

/// <summary>
/// Eğitim içeriği için kök aggregate. Roadmap (adımlar ve içerikler) bu aggregate içinde yönetilir.
/// </summary>
public sealed class Course : AggregateRoot
{
    private Course()
    {
    }

    public Course(string title, string description, Guid createdBy, string? thumbnailUrl = null)
    {
        Title = title;
        Description = description;
        ThumbnailUrl = thumbnailUrl;
        CreatedBy = createdBy;
        IsActive = true;
    }

    public string Title { get; private set; } = default!;
    public string Description { get; private set; } = default!;
    public string? ThumbnailUrl { get; private set; }
    public Guid CreatedBy { get; private set; }
    public bool IsActive { get; private set; } = true;

    /// <summary>
    /// Kursun öğrenme yolu. Tek roadmap kuralı aggregate içinde korunur.
    /// </summary>
    public CourseRoadmap? Roadmap { get; private set; }

    public void SetRoadmap(CourseRoadmap roadmap)
    {
        Roadmap = roadmap;
        UpdatedDate = DateTime.UtcNow;
    }

    public void UpdateDetails(string title, string description, string? thumbnailUrl)
    {
        Title = title;
        Description = description;
        ThumbnailUrl = thumbnailUrl;
        UpdatedDate = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedDate = DateTime.UtcNow;
    }
}

