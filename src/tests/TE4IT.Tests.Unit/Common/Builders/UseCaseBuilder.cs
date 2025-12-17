using TE4IT.Domain.Entities;
using TE4IT.Domain.ValueObjects;

namespace TE4IT.Tests.Unit.Common.Builders;

/// <summary>
/// Fluent API ile UseCase test data oluşturma builder'ı
/// </summary>
public class UseCaseBuilder
{
    private Guid _moduleId = Guid.NewGuid();
    private UserId _creatorId = new UserId(Guid.NewGuid());
    private string _title = "Test UseCase";
    private string? _description = "Test Description";
    private string? _importantNotes = null;
    private bool _isActive = true;

    public UseCaseBuilder WithModuleId(Guid moduleId)
    {
        _moduleId = moduleId;
        return this;
    }

    public UseCaseBuilder WithCreatorId(UserId creatorId)
    {
        _creatorId = creatorId;
        return this;
    }

    public UseCaseBuilder WithCreatorId(Guid creatorId)
    {
        _creatorId = new UserId(creatorId);
        return this;
    }

    public UseCaseBuilder WithTitle(string title)
    {
        _title = title;
        return this;
    }

    public UseCaseBuilder WithDescription(string? description)
    {
        _description = description;
        return this;
    }

    public UseCaseBuilder WithImportantNotes(string? importantNotes)
    {
        _importantNotes = importantNotes;
        return this;
    }

    public UseCaseBuilder WithIsActive(bool isActive)
    {
        _isActive = isActive;
        return this;
    }

    public UseCase Build()
    {
        var useCase = UseCase.Create(_moduleId, _creatorId, _title, _description, _importantNotes);
        if (!_isActive)
        {
            useCase.Archive();
        }
        return useCase;
    }

    public static UseCaseBuilder Create() => new();
}

