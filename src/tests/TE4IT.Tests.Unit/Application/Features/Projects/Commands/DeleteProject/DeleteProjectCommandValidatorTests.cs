using FluentAssertions;
using TE4IT.Application.Features.Projects.Commands.DeleteProject;
using Xunit;

namespace TE4IT.Tests.Unit.Application.Features.Projects.Commands.DeleteProject;

public class DeleteProjectCommandValidatorTests
{
    private readonly DeleteProjectCommandValidator _validator;

    public DeleteProjectCommandValidatorTests()
    {
        _validator = new DeleteProjectCommandValidator();
    }

    [Fact]
    public void Validate_WithValidCommand_ShouldPass()
    {
        // Arrange
        var command = new DeleteProjectCommand(Guid.NewGuid());

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WhenProjectIdIsEmpty_ShouldFail()
    {
        // Arrange
        var command = new DeleteProjectCommand(Guid.Empty);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "ProjectId");
    }
}

