namespace TE4IT.Domain.Events;

/// <summary>
/// Proje oluşturulduğunda fırlatılan domain event
/// </summary>
public class ProjectCreatedEvent : IDomainEvent
{
    public Guid EventId { get; }
    public DateTime OccurredAt { get; }
    public string EventType => nameof(ProjectCreatedEvent);

    public Guid ProjectId { get; }
    public Guid CreatorId { get; }
    public string ProjectTitle { get; }
    public string? ProjectDescription { get; }
    public DateTime CreatedAt { get; }

    public ProjectCreatedEvent(
        Guid projectId,
        Guid creatorId,
        string projectTitle,
        string? projectDescription)
    {
        EventId = Guid.NewGuid();
        OccurredAt = DateTime.UtcNow;
        CreatedAt = DateTime.UtcNow;

        ProjectId = projectId;
        CreatorId = creatorId;
        ProjectTitle = projectTitle;
        ProjectDescription = projectDescription;
    }
}
