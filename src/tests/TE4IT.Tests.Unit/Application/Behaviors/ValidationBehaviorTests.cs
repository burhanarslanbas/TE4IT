using FluentAssertions;
using FluentValidation;
using MediatR;
using Moq;
using TE4IT.Application.Behaviors;
using Xunit;

namespace TE4IT.Tests.Unit.Application.Behaviors;

public class ValidationBehaviorTests
{
    [Fact]
    public async Task Handle_WhenNoValidators_ProceedsToNext()
    {
        // Arrange
        var validators = new List<IValidator<TestRequest>>();
        var behavior = new ValidationBehavior<TestRequest, TestResponse>(validators);
        var request = new TestRequest { Value = "Test" };
        var nextCalled = false;
        RequestHandlerDelegate<TestResponse> next = () =>
        {
            nextCalled = true;
            return Task.FromResult(new TestResponse { Result = "Success" });
        };

        // Act
        var result = await behavior.Handle(request, next, CancellationToken.None);

        // Assert
        nextCalled.Should().BeTrue();
        result.Should().NotBeNull();
        result.Result.Should().Be("Success");
    }

    [Fact]
    public async Task Handle_WhenValidationPasses_ProceedsToNext()
    {
        // Arrange
        var validator = new TestRequestValidator();
        var validators = new List<IValidator<TestRequest>> { validator };
        var behavior = new ValidationBehavior<TestRequest, TestResponse>(validators);
        var request = new TestRequest { Value = "Valid" };
        var nextCalled = false;
        RequestHandlerDelegate<TestResponse> next = () =>
        {
            nextCalled = true;
            return Task.FromResult(new TestResponse { Result = "Success" });
        };

        // Act
        var result = await behavior.Handle(request, next, CancellationToken.None);

        // Assert
        nextCalled.Should().BeTrue();
        result.Should().NotBeNull();
        result.Result.Should().Be("Success");
    }

    [Fact]
    public async Task Handle_WhenValidationFails_ThrowsValidationException()
    {
        // Arrange
        var validator = new TestRequestValidator();
        var validators = new List<IValidator<TestRequest>> { validator };
        var behavior = new ValidationBehavior<TestRequest, TestResponse>(validators);
        var request = new TestRequest { Value = string.Empty }; // Invalid
        var nextCalled = false;
        RequestHandlerDelegate<TestResponse> next = () =>
        {
            nextCalled = true;
            return Task.FromResult(new TestResponse { Result = "Success" });
        };

        // Act
        var act = async () => await behavior.Handle(request, next, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
        nextCalled.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_WithMultipleValidators_AllMustPass()
    {
        // Arrange
        var validator1 = new TestRequestValidator();
        var validator2 = new TestRequestValidator2();
        var validators = new List<IValidator<TestRequest>> { validator1, validator2 };
        var behavior = new ValidationBehavior<TestRequest, TestResponse>(validators);
        var request = new TestRequest { Value = "Valid" };
        var nextCalled = false;
        RequestHandlerDelegate<TestResponse> next = () =>
        {
            nextCalled = true;
            return Task.FromResult(new TestResponse { Result = "Success" });
        };

        // Act
        var result = await behavior.Handle(request, next, CancellationToken.None);

        // Assert
        nextCalled.Should().BeTrue();
        result.Should().NotBeNull();
    }

    // Test classes
    public class TestRequest : IRequest<TestResponse>
    {
        public string Value { get; set; } = string.Empty;
    }

    public class TestResponse
    {
        public string Result { get; set; } = string.Empty;
    }

    public class TestRequestValidator : AbstractValidator<TestRequest>
    {
        public TestRequestValidator()
        {
            RuleFor(x => x.Value)
                .NotEmpty()
                .WithMessage("Value is required");
        }
    }

    public class TestRequestValidator2 : AbstractValidator<TestRequest>
    {
        public TestRequestValidator2()
        {
            RuleFor(x => x.Value)
                .MinimumLength(3)
                .WithMessage("Value must be at least 3 characters");
        }
    }
}

