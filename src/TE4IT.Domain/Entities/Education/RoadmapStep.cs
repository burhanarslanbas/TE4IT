using TE4IT.Domain.Entities.Common;

namespace TE4IT.Domain.Entities.Education;

/// <summary>
/// Roadmap içerisindeki adım. Zorunluluk ve sıralama kuralları adım seviyesinde tutulur.
/// </summary>
public sealed class RoadmapStep : BaseEntity
{
    private readonly List<CourseContent> contents = new();

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

    public string Title { get; private set; } = default!;
    public string? Description { get; private set; }
    public int Order { get; private set; }
    public bool IsRequired { get; private set; }
    public int EstimatedDurationMinutes { get; private set; }

    public IReadOnlyCollection<CourseContent> Contents => contents.AsReadOnly();

    public void SetContents(IEnumerable<CourseContent> stepContents)
    {
        var ordered = stepContents.OrderBy(c => c.Order).ToList();
        contents.Clear();
        contents.AddRange(ordered);
        UpdatedDate = DateTime.UtcNow;
    }
}

