using TE4IT.Domain.Entities.Common;

namespace TE4IT.Domain.Entities.Education;

/// <summary>
/// Roadmap içerisindeki adım. Zorunluluk ve sıralama kuralları adım seviyesinde tutulur.
/// </summary>
public sealed class RoadmapStep : BaseEntity
{
    private List<CourseContent> contents = new();

    private RoadmapStep()
    {
    }

    public RoadmapStep(string title, int order, bool isRequired, int estimatedDurationMinutes, string? description = null)
    {
        Title = title;
        Order = order;
        IsRequired = isRequired;
        EstimatedDurationMinutes = estimatedDurationMinutes;
        Description = description;
    }

    public string Title { get; set; } = default!;
    public string? Description { get; set; }
    public int Order { get; set; }
    public bool IsRequired { get; set; }
    public int EstimatedDurationMinutes { get; set; }

    public IReadOnlyCollection<CourseContent> Contents => contents.AsReadOnly();

    public void SetContents(IEnumerable<CourseContent> stepContents)
    {
        var ordered = stepContents.OrderBy(c => c.Order).ToList();
        contents.Clear();
        contents.AddRange(ordered);
        UpdatedDate = DateTime.UtcNow;
    }
}

