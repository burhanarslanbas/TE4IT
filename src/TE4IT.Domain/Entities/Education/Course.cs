using TE4IT.Domain.Entities.Common;
using TE4IT.Domain.Events;

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
        
        // Domain event fırlat
        AddDomainEvent(new CourseCreatedEvent(Id, createdBy, title));
    }

    public string Title { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string? ThumbnailUrl { get; set; }
    public Guid CreatedBy { get; set; }
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Kursun öğrenme yolu. Tek roadmap kuralı aggregate içinde korunur.
    /// </summary>
    public CourseRoadmap? Roadmap { get; set; }

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

