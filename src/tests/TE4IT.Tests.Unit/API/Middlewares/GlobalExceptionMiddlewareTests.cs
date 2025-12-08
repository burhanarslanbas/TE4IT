using System.Net;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using TE4IT.API.Middlewares;
using TE4IT.Domain.Exceptions.Auth;
using TE4IT.Domain.Exceptions.Common;
using TE4IT.Domain.Exceptions.Tasks;
using Xunit;

namespace TE4IT.Tests.Unit.API.Middlewares;

public class GlobalExceptionMiddlewareTests
{
    private readonly Mock<RequestDelegate> _nextMock;
    private readonly Mock<ILogger<GlobalExceptionMiddleware>> _loggerMock;
    private readonly GlobalExceptionMiddleware _middleware;
    private readonly DefaultHttpContext _httpContext;

    public GlobalExceptionMiddlewareTests()
    {
        _nextMock = new Mock<RequestDelegate>();
        _loggerMock = new Mock<ILogger<GlobalExceptionMiddleware>>();
        _middleware = new GlobalExceptionMiddleware(_nextMock.Object, _loggerMock.Object);
        _httpContext = new DefaultHttpContext();
        _httpContext.Request.Path = "/api/test";
        _httpContext.Response.Body = new MemoryStream();
    }

    [Fact]
    public async Task InvokeAsync_WithNoException_ProceedsToNext()
    {
        // Arrange
        _nextMock.Setup(x => x(It.IsAny<HttpContext>())).Returns(Task.CompletedTask);

        // Act
        await _middleware.InvokeAsync(_httpContext);

        // Assert
        _nextMock.Verify(x => x(It.IsAny<HttpContext>()), Times.Once);
        _httpContext.Response.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task HandleExceptionAsync_ValidationException_Returns400()
    {
        // Arrange
        var validationException = new ValidationException("Validation failed", new[]
        {
            new FluentValidation.Results.ValidationFailure("Title", "Title is required"),
            new FluentValidation.Results.ValidationFailure("Email", "Email is invalid")
        });
        _nextMock.Setup(x => x(It.IsAny<HttpContext>())).ThrowsAsync(validationException);

        // Act
        await _middleware.InvokeAsync(_httpContext);

        // Assert
        _httpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        _httpContext.Response.ContentType.Should().Be("application/json");
        
        var responseBody = await GetResponseBody();
        var response = JsonSerializer.Deserialize<JsonElement>(responseBody);
        response.GetProperty("statusCode").GetInt32().Should().Be(400);
        var messageObj = response.GetProperty("message");
        messageObj.GetProperty("message").GetString().Should().Be("Validation failed");
        messageObj.GetProperty("errors").GetArrayLength().Should().Be(2);
    }

    [Fact]
    public async Task HandleExceptionAsync_BusinessRuleViolation_Returns400()
    {
        // Arrange
        var exception = new BusinessRuleViolationException("Trial kullanıcı en fazla 1 proje oluşturabilir.");
        _nextMock.Setup(x => x(It.IsAny<HttpContext>())).ThrowsAsync(exception);

        // Act
        await _middleware.InvokeAsync(_httpContext);

        // Assert
        _httpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        var responseBody = await GetResponseBody();
        var response = JsonSerializer.Deserialize<JsonElement>(responseBody);
        response.GetProperty("statusCode").GetInt32().Should().Be(400);
        var messageObj = response.GetProperty("message");
        messageObj.GetProperty("message").GetString().Should().Contain("Trial kullanıcı");
    }

    [Fact]
    public async Task HandleExceptionAsync_ResourceNotFound_Returns404()
    {
        // Arrange
        var exception = new ResourceNotFoundException("Project", Guid.NewGuid());
        _nextMock.Setup(x => x(It.IsAny<HttpContext>())).ThrowsAsync(exception);

        // Act
        await _middleware.InvokeAsync(_httpContext);

        // Assert
        _httpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        var responseBody = await GetResponseBody();
        var response = JsonSerializer.Deserialize<JsonElement>(responseBody);
        response.GetProperty("statusCode").GetInt32().Should().Be(404);
    }

    [Fact]
    public async Task HandleExceptionAsync_UnauthorizedAccess_Returns401()
    {
        // Arrange
        var exception = new UnauthorizedAccessException("Unauthorized");
        _nextMock.Setup(x => x(It.IsAny<HttpContext>())).ThrowsAsync(exception);

        // Act
        await _middleware.InvokeAsync(_httpContext);

        // Assert
        _httpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.Unauthorized);
        var responseBody = await GetResponseBody();
        var response = JsonSerializer.Deserialize<JsonElement>(responseBody);
        response.GetProperty("statusCode").GetInt32().Should().Be(401);
    }

    [Fact]
    public async Task HandleExceptionAsync_InvalidCredentials_Returns401()
    {
        // Arrange
        var exception = new InvalidCredentialsException("test@example.com");
        _nextMock.Setup(x => x(It.IsAny<HttpContext>())).ThrowsAsync(exception);

        // Act
        await _middleware.InvokeAsync(_httpContext);

        // Assert
        _httpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.Unauthorized);
        var responseBody = await GetResponseBody();
        var response = JsonSerializer.Deserialize<JsonElement>(responseBody);
        response.GetProperty("statusCode").GetInt32().Should().Be(401);
    }

    [Fact]
    public async Task HandleExceptionAsync_UnknownException_Returns500()
    {
        // Arrange
        var exception = new InvalidOperationException("Unexpected error");
        _nextMock.Setup(x => x(It.IsAny<HttpContext>())).ThrowsAsync(exception);

        // Act
        await _middleware.InvokeAsync(_httpContext);

        // Assert
        _httpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
        var responseBody = await GetResponseBody();
        var response = JsonSerializer.Deserialize<JsonElement>(responseBody);
        response.GetProperty("statusCode").GetInt32().Should().Be(500);
        var messageObj = response.GetProperty("message");
        messageObj.GetProperty("message").GetString().Should().Contain("unexpected error");
    }

    [Fact]
    public async Task LogException_ValidationException_LogsAsWarning()
    {
        // Arrange
        var validationException = new ValidationException("Validation failed");
        _nextMock.Setup(x => x(It.IsAny<HttpContext>())).ThrowsAsync(validationException);

        // Act
        await _middleware.InvokeAsync(_httpContext);

        // Assert
        VerifyLog(LogLevel.Warning, "Validation failed");
    }

    [Fact]
    public async Task LogException_BusinessRuleViolation_LogsAsWarning()
    {
        // Arrange
        var exception = new BusinessRuleViolationException("Business rule violation");
        _nextMock.Setup(x => x(It.IsAny<HttpContext>())).ThrowsAsync(exception);

        // Act
        await _middleware.InvokeAsync(_httpContext);

        // Assert
        VerifyLog(LogLevel.Warning, "Business rule violation");
    }

    [Fact]
    public async Task LogException_ResourceNotFound_LogsAsInformation()
    {
        // Arrange
        var exception = new ResourceNotFoundException("Resource", Guid.NewGuid());
        _nextMock.Setup(x => x(It.IsAny<HttpContext>())).ThrowsAsync(exception);

        // Act
        await _middleware.InvokeAsync(_httpContext);

        // Assert
        VerifyLog(LogLevel.Information, "Resource not found");
    }

    [Fact]
    public async Task LogException_UnknownException_LogsAsError()
    {
        // Arrange
        var exception = new InvalidOperationException("Unexpected error");
        _nextMock.Setup(x => x(It.IsAny<HttpContext>())).ThrowsAsync(exception);

        // Act
        await _middleware.InvokeAsync(_httpContext);

        // Assert
        VerifyLog(LogLevel.Error, "Unhandled exception occurred");
    }

    [Fact]
    public async Task HandleExceptionAsync_ReturnsJsonResponse()
    {
        // Arrange
        var exception = new BusinessRuleViolationException("Test error");
        _nextMock.Setup(x => x(It.IsAny<HttpContext>())).ThrowsAsync(exception);

        // Act
        await _middleware.InvokeAsync(_httpContext);

        // Assert
        _httpContext.Response.ContentType.Should().Be("application/json");
        var responseBody = await GetResponseBody();
        responseBody.Should().NotBeNullOrEmpty();
        
        // Verify it's valid JSON
        var response = JsonSerializer.Deserialize<JsonElement>(responseBody);
        response.Should().NotBeNull();
    }

    [Fact]
    public async Task HandleExceptionAsync_ResponseContainsTimestamp()
    {
        // Arrange
        var exception = new BusinessRuleViolationException("Test error");
        _nextMock.Setup(x => x(It.IsAny<HttpContext>())).ThrowsAsync(exception);

        // Act
        await _middleware.InvokeAsync(_httpContext);

        // Assert
        var responseBody = await GetResponseBody();
        var response = JsonSerializer.Deserialize<JsonElement>(responseBody);
        response.GetProperty("timestamp").GetString().Should().NotBeNullOrEmpty();
        
        // Verify timestamp is valid DateTime
        var timestampStr = response.GetProperty("timestamp").GetString();
        var timestamp = DateTime.Parse(timestampStr!).ToUniversalTime();
        timestamp.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task HandleExceptionAsync_ResponseContainsPath()
    {
        // Arrange
        _httpContext.Request.Path = "/api/projects/123";
        var exception = new BusinessRuleViolationException("Test error");
        _nextMock.Setup(x => x(It.IsAny<HttpContext>())).ThrowsAsync(exception);

        // Act
        await _middleware.InvokeAsync(_httpContext);

        // Assert
        var responseBody = await GetResponseBody();
        var response = JsonSerializer.Deserialize<JsonElement>(responseBody);
        response.GetProperty("path").GetString().Should().Be("/api/projects/123");
    }

    [Fact]
    public async Task HandleExceptionAsync_ValidationException_ContainsFieldErrors()
    {
        // Arrange
        var validationException = new ValidationException("Validation failed", new[]
        {
            new FluentValidation.Results.ValidationFailure("Title", "Title is required"),
            new FluentValidation.Results.ValidationFailure("Email", "Email is invalid")
        });
        _nextMock.Setup(x => x(It.IsAny<HttpContext>())).ThrowsAsync(validationException);

        // Act
        await _middleware.InvokeAsync(_httpContext);

        // Assert
        var responseBody = await GetResponseBody();
        var response = JsonSerializer.Deserialize<JsonElement>(responseBody);
        response.GetProperty("statusCode").GetInt32().Should().Be(400);
        var messageObj = response.GetProperty("message");
        messageObj.GetProperty("message").GetString().Should().Be("Validation failed");
        var errors = messageObj.GetProperty("errors");
        errors.GetArrayLength().Should().Be(2);
        errors[0].GetProperty("field").GetString().Should().Be("Title");
        errors[0].GetProperty("message").GetString().Should().Be("Title is required");
        errors[1].GetProperty("field").GetString().Should().Be("Email");
        errors[1].GetProperty("message").GetString().Should().Be("Email is invalid");
    }

    [Fact]
    public async Task HandleExceptionAsync_DuplicateEmailException_Returns400()
    {
        // Arrange
        var exception = new DuplicateEmailException("test@example.com");
        _nextMock.Setup(x => x(It.IsAny<HttpContext>())).ThrowsAsync(exception);

        // Act
        await _middleware.InvokeAsync(_httpContext);

        // Assert
        _httpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        var responseBody = await GetResponseBody();
        var response = JsonSerializer.Deserialize<JsonElement>(responseBody);
        response.GetProperty("statusCode").GetInt32().Should().Be(400);
        var messageObj = response.GetProperty("message");
        messageObj.GetProperty("field").GetString().Should().Be("email");
    }

    [Fact]
    public async Task HandleExceptionAsync_DuplicateUserNameException_Returns400()
    {
        // Arrange
        var exception = new DuplicateUserNameException("testuser");
        _nextMock.Setup(x => x(It.IsAny<HttpContext>())).ThrowsAsync(exception);

        // Act
        await _middleware.InvokeAsync(_httpContext);

        // Assert
        _httpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        var responseBody = await GetResponseBody();
        var response = JsonSerializer.Deserialize<JsonElement>(responseBody);
        response.GetProperty("statusCode").GetInt32().Should().Be(400);
        var messageObj = response.GetProperty("message");
        messageObj.GetProperty("field").GetString().Should().Be("userName");
    }

    [Fact]
    public async Task HandleExceptionAsync_InvalidTaskStateTransition_Returns400()
    {
        // Arrange
        var exception = new InvalidTaskStateTransitionException("Cannot transition from Completed to InProgress");
        _nextMock.Setup(x => x(It.IsAny<HttpContext>())).ThrowsAsync(exception);

        // Act
        await _middleware.InvokeAsync(_httpContext);

        // Assert
        _httpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        var responseBody = await GetResponseBody();
        var response = JsonSerializer.Deserialize<JsonElement>(responseBody);
        response.GetProperty("statusCode").GetInt32().Should().Be(400);
    }

    private async Task<string> GetResponseBody()
    {
        _httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(_httpContext.Response.Body, Encoding.UTF8, leaveOpen: true);
        return await reader.ReadToEndAsync();
    }

    private void VerifyLog(LogLevel logLevel, string expectedMessage)
    {
        _loggerMock.Verify(
            x => x.Log(
                logLevel,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains(expectedMessage)),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}

