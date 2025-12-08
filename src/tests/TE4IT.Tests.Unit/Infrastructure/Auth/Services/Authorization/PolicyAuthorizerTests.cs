using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Moq;
using TE4IT.Application.Abstractions.Auth;
using TE4IT.Infrastructure.Auth.Services.Authorization;
using Xunit;

namespace TE4IT.Tests.Unit.Infrastructure.Auth.Services.Authorization;

public class PolicyAuthorizerTests
{
    private readonly Mock<IAuthorizationService> _authorizationServiceMock;
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private readonly PolicyAuthorizer _authorizer;

    public PolicyAuthorizerTests()
    {
        _authorizationServiceMock = new Mock<IAuthorizationService>();
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        _authorizer = new PolicyAuthorizer(_authorizationServiceMock.Object, _httpContextAccessorMock.Object);
    }

    [Fact]
    public async Task AuthorizeAsync_WithValidPolicy_ReturnsTrue()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())
        };
        httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(claims, "Test"));
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

        _authorizationServiceMock.Setup(x => x.AuthorizeAsync(
            It.IsAny<ClaimsPrincipal>(), null, "ProjectCreate"))
            .ReturnsAsync(AuthorizationResult.Success());

        // Act
        var result = await _authorizer.AuthorizeAsync("ProjectCreate", CancellationToken.None);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task AuthorizeAsync_WithInvalidPolicy_ReturnsFalse()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())
        };
        httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(claims, "Test"));
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

        _authorizationServiceMock.Setup(x => x.AuthorizeAsync(
            It.IsAny<ClaimsPrincipal>(), null, "ProjectDelete"))
            .ReturnsAsync(AuthorizationResult.Failed());

        // Act
        var result = await _authorizer.AuthorizeAsync("ProjectDelete", CancellationToken.None);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task AuthorizeAsync_WithNullHttpContext_ReturnsFalse()
    {
        // Arrange
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns((HttpContext?)null);

        // Act
        var result = await _authorizer.AuthorizeAsync("ProjectCreate", CancellationToken.None);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task AuthorizeAsync_WithMissingClaims_ReturnsFalse()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();
        httpContext.User = new ClaimsPrincipal(new ClaimsIdentity()); // No claims
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

        _authorizationServiceMock.Setup(x => x.AuthorizeAsync(
            It.IsAny<ClaimsPrincipal>(), null, "ProjectCreate"))
            .ReturnsAsync(AuthorizationResult.Failed());

        // Act
        var result = await _authorizer.AuthorizeAsync("ProjectCreate", CancellationToken.None);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task AuthorizeAsync_WithCorrectRole_ReturnsTrue()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
            new(ClaimTypes.Role, "Administrator")
        };
        httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(claims, "Test"));
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

        _authorizationServiceMock.Setup(x => x.AuthorizeAsync(
            It.IsAny<ClaimsPrincipal>(), null, "ProjectCreate"))
            .ReturnsAsync(AuthorizationResult.Success());

        // Act
        var result = await _authorizer.AuthorizeAsync("ProjectCreate", CancellationToken.None);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task AuthorizeAsync_WithCorrectPermission_ReturnsTrue()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
            new("permission", "Project.Create")
        };
        httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(claims, "Test"));
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

        _authorizationServiceMock.Setup(x => x.AuthorizeAsync(
            It.IsAny<ClaimsPrincipal>(), null, "ProjectCreate"))
            .ReturnsAsync(AuthorizationResult.Success());

        // Act
        var result = await _authorizer.AuthorizeAsync("ProjectCreate", CancellationToken.None);

        // Assert
        result.Should().BeTrue();
    }
}

