using FluentAssertions;
using TE4IT.Domain.Exceptions.Auth;
using TE4IT.Domain.Exceptions.Base;
using TE4IT.Domain.Exceptions.Common;
using TE4IT.Domain.Exceptions.Projects;
using TE4IT.Domain.Exceptions.Tasks;
using Xunit;

namespace TE4IT.Tests.Unit.Domain.Exceptions;

public class DomainExceptionTests
{
    [Fact]
    public void BusinessRuleViolationException_ContainsMessage()
    {
        // Arrange
        var message = "Trial kullanıcı en fazla 1 proje oluşturabilir.";

        // Act
        var exception = new BusinessRuleViolationException(message);

        // Assert
        exception.Message.Should().Be(message);
        exception.Should().BeAssignableTo<DomainException>();
    }

    [Fact]
    public void ResourceNotFoundException_ContainsMessage()
    {
        // Arrange
        var resourceName = "Project";
        var resourceId = Guid.NewGuid();

        // Act
        var exception = new ResourceNotFoundException(resourceName, resourceId);

        // Assert
        exception.Message.Should().Contain(resourceName);
        exception.Message.Should().Contain(resourceId.ToString());
        exception.Should().BeAssignableTo<DomainException>();
    }

    [Fact]
    public void InvalidCredentialsException_ContainsMessage()
    {
        // Arrange
        var email = "test@example.com";

        // Act
        var exception = new InvalidCredentialsException(email);

        // Assert
        exception.Message.Should().Contain(email);
        exception.Should().BeAssignableTo<DomainException>();
    }

    [Fact]
    public void DuplicateEmailException_ContainsMessage()
    {
        // Arrange
        var email = "test@example.com";

        // Act
        var exception = new DuplicateEmailException(email);

        // Assert
        exception.Message.Should().Contain(email);
        exception.Should().BeAssignableTo<DomainException>();
    }

    [Fact]
    public void DuplicateUserNameException_ContainsMessage()
    {
        // Arrange
        var userName = "testuser";

        // Act
        var exception = new DuplicateUserNameException(userName);

        // Assert
        exception.Message.Should().Contain(userName);
        exception.Should().BeAssignableTo<DomainException>();
    }

    [Fact]
    public void UserRegistrationFailedException_ContainsMessage()
    {
        // Arrange
        var message = "Password does not meet requirements";

        // Act
        var exception = new UserRegistrationFailedException(message);

        // Assert
        exception.Message.Should().Be($"Kullanıcı kaydı başarısız: {message}");
        exception.Reason.Should().Be(message);
        exception.Should().BeAssignableTo<DomainException>();
    }

    [Fact]
    public void InvalidTaskStateTransitionException_ContainsMessage()
    {
        // Arrange
        var message = "Cannot transition from Completed to InProgress";

        // Act
        var exception = new InvalidTaskStateTransitionException(message);

        // Assert
        exception.Message.Should().Be(message);
        exception.Should().BeAssignableTo<DomainException>();
    }

    [Fact]
    public void TaskDependencyViolationException_ContainsMessage()
    {
        // Arrange
        var message = "Task has dependent tasks that must be completed first";

        // Act
        var exception = new TaskDependencyViolationException(message);

        // Assert
        exception.Message.Should().Be(message);
        exception.Should().BeAssignableTo<DomainException>();
    }

    [Fact]
    public void ProjectAccessDeniedException_ContainsMessage()
    {
        // Arrange
        var message = "Access denied to project";

        // Act
        var exception = new ProjectAccessDeniedException(message);

        // Assert
        exception.Message.Should().Be(message);
        exception.Should().BeAssignableTo<DomainException>();
    }
}
