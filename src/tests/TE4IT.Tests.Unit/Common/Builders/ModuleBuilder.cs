using TE4IT.Domain.Entities;
using TE4IT.Domain.ValueObjects;

namespace TE4IT.Tests.Unit.Common.Builders;

/// <summary>
/// Fluent API ile Module test data oluşturma builder'ı
/// </summary>
public class ModuleBuilder
{
    private Guid _projectId = Guid.NewGuid();
    private UserId _creatorId = new UserId(Guid.NewGuid());
    private string _title = "Test Module";
    private string? _description = "Test Description";
    private bool _isActive = true;

    public ModuleBuilder WithProjectId(Guid projectId)
    {
        _projectId = projectId;
        return this;
    }

    public ModuleBuilder WithCreatorId(UserId creatorId)
    {
        _creatorId = creatorId;
        return this;
    }

    public ModuleBuilder WithCreatorId(Guid creatorId)
    {
        _creatorId = new UserId(creatorId);
        return this;
    }

    public ModuleBuilder WithTitle(string title)
    {
        _title = title;
        return this;
    }

    public ModuleBuilder WithDescription(string? description)
    {
        _description = description;
        return this;
    }

    public ModuleBuilder WithIsActive(bool isActive)
    {
        _isActive = isActive;
        return this;
    }

    public Module Build()
    {
        var module = Module.Create(_projectId, _creatorId, _title, _description);
        if (!_isActive)
        {
            module.Archive();
        }
        return module;
    }

    public static ModuleBuilder Create() => new();
}

