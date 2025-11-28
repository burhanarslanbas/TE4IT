using FluentAssertions;
using TE4IT.Application.Features.Projects.Commands.ChangeProjectStatus;
using Xunit;

namespace TE4IT.Tests.Unit.Application.Features.Projects.Commands.ChangeProjectStatus;

public class ChangeProjectStatusCommandValidatorTests
{
    private readonly ChangeProjectStatusCommandValidator _validator;

    public ChangeProjectStatusCommandValidatorTests()
    {
        _validator = new ChangeProjectStatusCommandValidator();
    }

    [Fact]
    public void Validate_WithValidCommand_ShouldPass()
    {
        // Arrange
        var command = new ChangeProjectStatusCommand(Guid.NewGuid(), true);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WhenProjectIdIsEmpty_ShouldFail()
    {
        // Arrange
        var command = new ChangeProjectStatusCommand(Guid.Empty, true);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "ProjectId");
    }

    [Fact]
    public void Validate_WithIsActiveFalse_ShouldPass()
    {
        // Arrange
        var command = new ChangeProjectStatusCommand(Guid.NewGuid(), false);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }
}

