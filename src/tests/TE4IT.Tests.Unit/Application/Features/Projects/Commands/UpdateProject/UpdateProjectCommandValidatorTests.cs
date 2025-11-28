using FluentAssertions;
using TE4IT.Application.Features.Projects.Commands.UpdateProject;
using TE4IT.Domain.Constants;
using Xunit;

namespace TE4IT.Tests.Unit.Application.Features.Projects.Commands.UpdateProject;

public class UpdateProjectCommandValidatorTests
{
    private readonly UpdateProjectCommandValidator _validator;

    public UpdateProjectCommandValidatorTests()
    {
        _validator = new UpdateProjectCommandValidator();
    }

    [Fact]
    public void Validate_WithValidCommand_ShouldPass()
    {
        // Arrange
        var command = new UpdateProjectCommand(Guid.NewGuid(), "Valid Title", "Valid Description");

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WhenProjectIdIsEmpty_ShouldFail()
    {
        // Arrange
        var command = new UpdateProjectCommand(Guid.Empty, "Valid Title", "Description");

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "ProjectId");
    }

    [Fact]
    public void Validate_WhenTitleIsEmpty_ShouldFail()
    {
        // Arrange
        var command = new UpdateProjectCommand(Guid.NewGuid(), string.Empty, "Description");

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Title");
    }

    [Fact]
    public void Validate_WhenTitleTooShort_ShouldFail()
    {
        // Arrange
        var shortTitle = new string('A', 2); // Minimum is 3
        var command = new UpdateProjectCommand(Guid.NewGuid(), shortTitle, "Description");

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Title");
    }

    [Fact]
    public void Validate_WhenTitleTooLong_ShouldFail()
    {
        // Arrange
        var longTitle = new string('A', 201); // Maximum is 200
        var command = new UpdateProjectCommand(Guid.NewGuid(), longTitle, "Description");

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Title");
    }

    [Fact]
    public void Validate_WhenDescriptionTooLong_ShouldFail()
    {
        // Arrange
        var longDescription = new string('A', 4001); // Maximum is 4000
        var command = new UpdateProjectCommand(Guid.NewGuid(), "Valid Title", longDescription);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Description");
    }

    [Fact]
    public void Validate_WhenDescriptionIsNull_ShouldPass()
    {
        // Arrange
        var command = new UpdateProjectCommand(Guid.NewGuid(), "Valid Title", null);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }
}

