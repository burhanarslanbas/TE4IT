using TE4IT.Domain.Entities.Common;

namespace TE4IT.Domain.Entities.Education;

/// <summary>
/// Kursa ait öğrenme yolu. Adımlar ve içerikler aggregate içinde sıralı biçimde tutulur.
/// </summary>
public sealed class CourseRoadmap : BaseEntity
{
    private List<RoadmapStep> steps = new();

    private CourseRoadmap()
    {
    }

    public CourseRoadmap(string title, int estimatedDurationMinutes, string? description = null)
    {
        Title = title;
        EstimatedDurationMinutes = estimatedDurationMinutes;
        Description = description;
    }

    public string Title { get; set; } = default!;
    public string? Description { get; set; }
    public int EstimatedDurationMinutes { get; set; }

    public IReadOnlyCollection<RoadmapStep> Steps => steps.AsReadOnly();

    public void SetSteps(IEnumerable<RoadmapStep> roadmapSteps)
    {
        var ordered = roadmapSteps.OrderBy(s => s.Order).ToList();
        steps.Clear();
        steps.AddRange(ordered);
        UpdatedDate = DateTime.UtcNow;
    }
}

