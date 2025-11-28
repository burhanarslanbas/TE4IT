using FluentAssertions;
using FluentValidation;
using TE4IT.Application.Features.Projects.Commands.CreateProject;
using TE4IT.Domain.Constants;
using Xunit;

namespace TE4IT.Tests.Unit.Application.Features.Projects.Commands.CreateProject;

public class CreateProjectCommandValidatorTests
{
    private readonly CreateProjectCommandValidator _validator;

    public CreateProjectCommandValidatorTests()
    {
        _validator = new CreateProjectCommandValidator();
    }

    [Fact]
    public void Validate_WithValidCommand_ShouldPass()
    {
        // Arrange
        var command = new CreateProjectCommand("Valid Project Title", "Valid Description");

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WhenTitleIsEmpty_ShouldFail()
    {
        // Arrange
        var command = new CreateProjectCommand(string.Empty, "Description");

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Title" && e.ErrorMessage.Contains("zorunludur"));
    }

    [Fact]
    public void Validate_WhenTitleTooShort_ShouldFail()
    {
        // Arrange
        var shortTitle = new string('A', DomainConstants.MinProjectTitleLength - 1);
        var command = new CreateProjectCommand(shortTitle, "Description");

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Title" && e.ErrorMessage.Contains($"en az {DomainConstants.MinProjectTitleLength} karakter"));
    }

    [Fact]
    public void Validate_WhenTitleTooLong_ShouldFail()
    {
        // Arrange
        var longTitle = new string('A', DomainConstants.MaxProjectTitleLength + 1);
        var command = new CreateProjectCommand(longTitle, "Description");

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Title" && e.ErrorMessage.Contains($"en fazla {DomainConstants.MaxProjectTitleLength} karakter"));
    }

    [Fact]
    public void Validate_WhenDescriptionTooLong_ShouldFail()
    {
        // Arrange
        var longDescription = new string('A', DomainConstants.MaxProjectDescriptionLength + 1);
        var command = new CreateProjectCommand("Valid Title", longDescription);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Description" && e.ErrorMessage.Contains($"en fazla {DomainConstants.MaxProjectDescriptionLength} karakter"));
    }

    [Fact]
    public void Validate_WhenDescriptionIsNull_ShouldPass()
    {
        // Arrange
        var command = new CreateProjectCommand("Valid Title", null);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WhenTitleIsMinimumLength_ShouldPass()
    {
        // Arrange
        var minTitle = new string('A', DomainConstants.MinProjectTitleLength);
        var command = new CreateProjectCommand(minTitle, "Description");

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WhenTitleIsMaximumLength_ShouldPass()
    {
        // Arrange
        var maxTitle = new string('A', DomainConstants.MaxProjectTitleLength);
        var command = new CreateProjectCommand(maxTitle, "Description");

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WhenDescriptionIsMaximumLength_ShouldPass()
    {
        // Arrange
        var maxDescription = new string('A', DomainConstants.MaxProjectDescriptionLength);
        var command = new CreateProjectCommand("Valid Title", maxDescription);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }
}

