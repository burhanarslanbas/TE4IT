using Project = TE4IT.Domain.Entities.Project;
using TE4IT.Domain.ValueObjects;

namespace TE4IT.Tests.Unit.Common.Builders;

/// <summary>
/// Fluent API ile Project test data oluşturma builder'ı
/// </summary>
public class ProjectBuilder
{
    private UserId _creatorId = new UserId(Guid.NewGuid());
    private string _title = "Test Project";
    private string? _description = "Test Description";
    private bool _isActive = true;

    public ProjectBuilder WithCreatorId(UserId creatorId)
    {
        _creatorId = creatorId;
        return this;
    }

    public ProjectBuilder WithCreatorId(Guid creatorId)
    {
        _creatorId = new UserId(creatorId);
        return this;
    }

    public ProjectBuilder WithTitle(string title)
    {
        _title = title;
        return this;
    }

    public ProjectBuilder WithDescription(string? description)
    {
        _description = description;
        return this;
    }

    public ProjectBuilder WithIsActive(bool isActive)
    {
        _isActive = isActive;
        return this;
    }

    public Project Build()
    {
        var project = Project.Create(_creatorId, _title, _description);
        if (!_isActive)
        {
            project.Archive();
        }
        return project;
    }

    public static ProjectBuilder Create() => new();
}

