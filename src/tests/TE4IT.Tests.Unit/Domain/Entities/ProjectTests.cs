using System.Threading.Tasks;
using FluentAssertions;
using TE4IT.Domain.Constants;
using Project = TE4IT.Domain.Entities.Project;
using TE4IT.Domain.Events;
using TE4IT.Domain.ValueObjects;
using TE4IT.Tests.Unit.Common.Builders;
using Xunit;

namespace TE4IT.Tests.Unit.Domain.Entities;

public class ProjectTests
{
    [Fact]
    public void Create_WithValidParameters_ReturnsProject()
    {
        // Arrange
        var creatorId = new UserId(Guid.NewGuid());
        var title = "Test Project";
        var description = "Test Description";

        // Act
        var project = Project.Create(creatorId, title, description);

        // Assert
        project.Should().NotBeNull();
        project.CreatorId.Should().Be(creatorId);
        project.Title.Should().Be(title);
        project.Description.Should().Be(description);
        project.IsActive.Should().BeTrue();
        project.StartedDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Create_WhenTitleIsEmpty_ThrowsArgumentException()
    {
        // Arrange
        var creatorId = new UserId(Guid.NewGuid());

        // Act
        var act = () => Project.Create(creatorId, string.Empty);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Bu alan zorunludur*");
    }

    [Fact]
    public void Create_WhenTitleIsWhitespace_ThrowsArgumentException()
    {
        // Arrange
        var creatorId = new UserId(Guid.NewGuid());

        // Act
        var act = () => Project.Create(creatorId, "   ");

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_WhenTitleTooShort_ThrowsArgumentException()
    {
        // Arrange
        var creatorId = new UserId(Guid.NewGuid());
        var shortTitle = new string('A', DomainConstants.MinProjectTitleLength - 1);

        // Act
        var act = () => Project.Create(creatorId, shortTitle);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage($"*en az {DomainConstants.MinProjectTitleLength} karakter*");
    }

    [Fact]
    public void Create_WhenTitleTooLong_ThrowsArgumentException()
    {
        // Arrange
        var creatorId = new UserId(Guid.NewGuid());
        var longTitle = new string('A', DomainConstants.MaxProjectTitleLength + 1);

        // Act
        var act = () => Project.Create(creatorId, longTitle);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage($"*en fazla {DomainConstants.MaxProjectTitleLength} karakter*");
    }

    [Fact]
    public void Create_WhenDescriptionTooLong_ThrowsArgumentException()
    {
        // Arrange
        var creatorId = new UserId(Guid.NewGuid());
        var title = "Test Project";
        var longDescription = new string('A', DomainConstants.MaxProjectDescriptionLength + 1);

        // Act
        var act = () => Project.Create(creatorId, title, longDescription);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage($"*en fazla {DomainConstants.MaxProjectDescriptionLength} karakter*");
    }

    [Fact]
    public void Create_SetsDefaultValues()
    {
        // Arrange
        var creatorId = new UserId(Guid.NewGuid());
        var title = "Test Project";

        // Act
        var project = Project.Create(creatorId, title);

        // Assert
        project.IsActive.Should().BeTrue();
        project.StartedDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Create_RaisesProjectCreatedEvent()
    {
        // Arrange
        var creatorId = new UserId(Guid.NewGuid());
        var title = "Test Project";
        var description = "Test Description";

        // Act
        var project = Project.Create(creatorId, title, description);

        // Assert
        project.DomainEvents.Should().HaveCount(1);
        project.DomainEvents.Should().ContainSingle(e => e is ProjectCreatedEvent);
        var @event = project.DomainEvents.OfType<ProjectCreatedEvent>().Single();
        @event.ProjectId.Should().Be(project.Id);
        @event.CreatorId.Should().Be(creatorId.Value);
        @event.ProjectTitle.Should().Be(title);
        @event.ProjectDescription.Should().Be(description);
    }

    [Fact]
    public void UpdateTitle_WithValidTitle_UpdatesTitle()
    {
        // Arrange
        var project = ProjectBuilder.Create()
            .WithTitle("Old Title")
            .Build();
        var newTitle = "New Title";

        // Act
        project.UpdateTitle(newTitle);

        // Assert
        project.Title.Should().Be(newTitle);
    }

    [Fact]
    public void UpdateTitle_WhenTitleIsEmpty_ThrowsArgumentException()
    {
        // Arrange
        var project = ProjectBuilder.Create().Build();

        // Act
        var act = () => project.UpdateTitle(string.Empty);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void UpdateTitle_WhenTitleTooShort_ThrowsArgumentException()
    {
        // Arrange
        var project = ProjectBuilder.Create().Build();
        var shortTitle = new string('A', DomainConstants.MinProjectTitleLength - 1);

        // Act
        var act = () => project.UpdateTitle(shortTitle);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void UpdateTitle_WhenTitleTooLong_ThrowsArgumentException()
    {
        // Arrange
        var project = ProjectBuilder.Create().Build();
        var longTitle = new string('A', DomainConstants.MaxProjectTitleLength + 1);

        // Act
        var act = () => project.UpdateTitle(longTitle);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void UpdateDescription_WithValidDescription_UpdatesDescription()
    {
        // Arrange
        var project = ProjectBuilder.Create()
            .WithDescription("Old Description")
            .Build();
        var newDescription = "New Description";

        // Act
        project.UpdateDescription(newDescription);

        // Assert
        project.Description.Should().Be(newDescription);
    }

    [Fact]
    public void UpdateDescription_WithNull_UpdatesToNull()
    {
        // Arrange
        var project = ProjectBuilder.Create()
            .WithDescription("Old Description")
            .Build();

        // Act
        project.UpdateDescription(null);

        // Assert
        project.Description.Should().BeNull();
    }

    [Fact]
    public void UpdateDescription_WhenDescriptionTooLong_ThrowsArgumentException()
    {
        // Arrange
        var project = ProjectBuilder.Create().Build();
        var longDescription = new string('A', DomainConstants.MaxProjectDescriptionLength + 1);

        // Act
        var act = () => project.UpdateDescription(longDescription);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void ChangeStatus_WhenStatusChanges_RaisesEvent()
    {
        // Arrange
        var project = ProjectBuilder.Create()
            .WithIsActive(true)
            .Build();
        project.ClearDomainEvents();

        // Act
        project.ChangeStatus(false);

        // Assert
        project.IsActive.Should().BeFalse();
        project.DomainEvents.Should().HaveCount(1);
        project.DomainEvents.Should().ContainSingle(e => e is ProjectStatusChangedEvent);
        var @event = project.DomainEvents.OfType<ProjectStatusChangedEvent>().Single();
        @event.PreviousStatus.Should().BeTrue();
        @event.NewStatus.Should().BeFalse();
    }

    [Fact]
    public void ChangeStatus_WhenStatusSame_DoesNotRaiseEvent()
    {
        // Arrange
        var project = ProjectBuilder.Create()
            .WithIsActive(true)
            .Build();
        project.ClearDomainEvents();

        // Act
        project.ChangeStatus(true);

        // Assert
        project.IsActive.Should().BeTrue();
        project.DomainEvents.Should().BeEmpty();
    }

    [Fact]
    public void Archive_SetsIsActiveToFalse()
    {
        // Arrange
        var project = ProjectBuilder.Create()
            .WithIsActive(true)
            .Build();
        project.ClearDomainEvents();

        // Act
        project.Archive();

        // Assert
        project.IsActive.Should().BeFalse();
        project.DomainEvents.Should().HaveCount(1);
        project.DomainEvents.Should().ContainSingle(e => e is ProjectStatusChangedEvent);
    }

    [Fact]
    public void Activate_SetsIsActiveToTrue()
    {
        // Arrange
        var project = ProjectBuilder.Create()
            .WithIsActive(false)
            .Build();
        project.ClearDomainEvents();

        // Act
        project.Activate();

        // Assert
        project.IsActive.Should().BeTrue();
        project.DomainEvents.Should().HaveCount(1);
        project.DomainEvents.Should().ContainSingle(e => e is ProjectStatusChangedEvent);
    }
}

