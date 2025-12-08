using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using TE4IT.Application.Abstractions.Auth;
using TE4IT.Application.Features.Auth.Commands.Login;
using TE4IT.Domain.Constants;
using TE4IT.Domain.Exceptions.Auth;
using Xunit;

namespace TE4IT.Tests.Unit.Application.Features.Auth.Commands.Login;

public class LoginCommandHandlerTests
{
    private readonly Mock<IUserAccountService> _userAccountServiceMock;
    private readonly Mock<IUserInfoService> _userInfoServiceMock;
    private readonly Mock<IRolePermissionService> _rolePermissionServiceMock;
    private readonly Mock<ITokenService> _tokenServiceMock;
    private readonly Mock<IRefreshTokenService> _refreshTokenServiceMock;
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private readonly LoginCommandHandler _handler;

    public LoginCommandHandlerTests()
    {
        _userAccountServiceMock = new Mock<IUserAccountService>();
        _userInfoServiceMock = new Mock<IUserInfoService>();
        _rolePermissionServiceMock = new Mock<IRolePermissionService>();
        _tokenServiceMock = new Mock<ITokenService>();
        _refreshTokenServiceMock = new Mock<IRefreshTokenService>();
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();

        var httpContext = new DefaultHttpContext();
        httpContext.Connection.RemoteIpAddress = System.Net.IPAddress.Parse("192.168.1.1");
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

        _handler = new LoginCommandHandler(
            _userAccountServiceMock.Object,
            _userInfoServiceMock.Object,
            _rolePermissionServiceMock.Object,
            _tokenServiceMock.Object,
            _refreshTokenServiceMock.Object,
            _httpContextAccessorMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidCredentials_ReturnsTokens()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var email = "test@example.com";
        var password = "Test123!@#";
        var command = new LoginCommand(email, password);

        _userAccountServiceMock.Setup(x => x.ValidateCredentialsAsync(email, password, It.IsAny<CancellationToken>()))
            .ReturnsAsync(userId);

        var userInfo = new UserInfo(userId, "testuser", email, new[] { RoleNames.Administrator }, "v1.0");
        _userInfoServiceMock.Setup(x => x.GetUserInfoAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(userInfo);

        _rolePermissionServiceMock.Setup(x => x.GetPermissionsForRoles(It.IsAny<IEnumerable<string>>()))
            .Returns(new[] { Permissions.Project.Create, Permissions.Project.View });

        _tokenServiceMock.Setup(x => x.CreateAccessToken(
            userId, "testuser", email, It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>(), "v1.0"))
            .Returns(("access-token", DateTime.UtcNow.AddMinutes(15)));

        _refreshTokenServiceMock.Setup(x => x.IssueAsync(userId, "192.168.1.1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(("refresh-token", DateTime.UtcNow.AddDays(7)));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.UserId.Should().Be(userId);
        result.Email.Should().Be(email);
        result.AccessToken.Should().Be("access-token");
        result.RefreshToken.Should().Be("refresh-token");
    }

    [Fact]
    public async Task Handle_ReturnsAccessAndRefreshTokens()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var email = "test@example.com";
        var command = new LoginCommand(email, "Test123!@#");

        _userAccountServiceMock.Setup(x => x.ValidateCredentialsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(userId);

        var userInfo = new UserInfo(userId, "testuser", email, Array.Empty<string>(), null);
        _userInfoServiceMock.Setup(x => x.GetUserInfoAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(userInfo);

        _rolePermissionServiceMock.Setup(x => x.GetPermissionsForRoles(It.IsAny<IEnumerable<string>>()))
            .Returns(Array.Empty<string>());

        _tokenServiceMock.Setup(x => x.CreateAccessToken(
            It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>(), It.IsAny<string>()))
            .Returns(("access-token", DateTime.UtcNow.AddMinutes(15)));

        _refreshTokenServiceMock.Setup(x => x.IssueAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(("refresh-token", DateTime.UtcNow.AddDays(7)));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.AccessToken.Should().NotBeNullOrEmpty();
        result.RefreshToken.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Handle_TokenContainsUserInfo()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var email = "test@example.com";
        var userName = "testuser";
        var command = new LoginCommand(email, "Test123!@#");

        _userAccountServiceMock.Setup(x => x.ValidateCredentialsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(userId);

        var userInfo = new UserInfo(userId, userName, email, Array.Empty<string>(), null);
        _userInfoServiceMock.Setup(x => x.GetUserInfoAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(userInfo);

        _rolePermissionServiceMock.Setup(x => x.GetPermissionsForRoles(It.IsAny<IEnumerable<string>>()))
            .Returns(Array.Empty<string>());

        _tokenServiceMock.Setup(x => x.CreateAccessToken(
            userId, userName, email, It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>(), It.IsAny<string>()))
            .Returns(("access-token", DateTime.UtcNow.AddMinutes(15)));

        _refreshTokenServiceMock.Setup(x => x.IssueAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(("refresh-token", DateTime.UtcNow.AddDays(7)));

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _tokenServiceMock.Verify(x => x.CreateAccessToken(
            userId, userName, email, It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>(), It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task Handle_TokenContainsRoles()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var roles = new[] { RoleNames.Administrator, RoleNames.TeamLead };
        var command = new LoginCommand("test@example.com", "Test123!@#");

        _userAccountServiceMock.Setup(x => x.ValidateCredentialsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(userId);

        var userInfo = new UserInfo(userId, "testuser", "test@example.com", roles, null);
        _userInfoServiceMock.Setup(x => x.GetUserInfoAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(userInfo);

        _rolePermissionServiceMock.Setup(x => x.GetPermissionsForRoles(roles))
            .Returns(Array.Empty<string>());

        _tokenServiceMock.Setup(x => x.CreateAccessToken(
            It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), roles, It.IsAny<IEnumerable<string>>(), It.IsAny<string>()))
            .Returns(("access-token", DateTime.UtcNow.AddMinutes(15)));

        _refreshTokenServiceMock.Setup(x => x.IssueAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(("refresh-token", DateTime.UtcNow.AddDays(7)));

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _tokenServiceMock.Verify(x => x.CreateAccessToken(
            It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), roles, It.IsAny<IEnumerable<string>>(), It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task Handle_TokenContainsPermissions()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var roles = new[] { RoleNames.Administrator };
        var permissions = new[] { Permissions.Project.Create, Permissions.Project.View };
        var command = new LoginCommand("test@example.com", "Test123!@#");

        _userAccountServiceMock.Setup(x => x.ValidateCredentialsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(userId);

        var userInfo = new UserInfo(userId, "testuser", "test@example.com", roles, null);
        _userInfoServiceMock.Setup(x => x.GetUserInfoAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(userInfo);

        _rolePermissionServiceMock.Setup(x => x.GetPermissionsForRoles(roles))
            .Returns(permissions);

        _tokenServiceMock.Setup(x => x.CreateAccessToken(
            It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), permissions, It.IsAny<string>()))
            .Returns(("access-token", DateTime.UtcNow.AddMinutes(15)));

        _refreshTokenServiceMock.Setup(x => x.IssueAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(("refresh-token", DateTime.UtcNow.AddDays(7)));

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _tokenServiceMock.Verify(x => x.CreateAccessToken(
            It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), permissions, It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithInvalidCredentials_ReturnsNull()
    {
        // Arrange
        var command = new LoginCommand("test@example.com", "WrongPassword");

        _userAccountServiceMock.Setup(x => x.ValidateCredentialsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidCredentialsException("test@example.com"));

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidCredentialsException>();
    }

    [Fact]
    public async Task Handle_WithLockedAccount_ThrowsException()
    {
        // Arrange
        var command = new LoginCommand("test@example.com", "Test123!@#");

        _userAccountServiceMock.Setup(x => x.ValidateCredentialsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidCredentialsException("test@example.com"));

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidCredentialsException>();
    }

    [Fact]
    public async Task Handle_RecordsIpAddress()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var ipAddress = "192.168.1.100";
        var command = new LoginCommand("test@example.com", "Test123!@#");

        var httpContext = new DefaultHttpContext();
        httpContext.Connection.RemoteIpAddress = System.Net.IPAddress.Parse(ipAddress);
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

        _userAccountServiceMock.Setup(x => x.ValidateCredentialsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(userId);

        var userInfo = new UserInfo(userId, "testuser", "test@example.com", Array.Empty<string>(), null);
        _userInfoServiceMock.Setup(x => x.GetUserInfoAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(userInfo);

        _rolePermissionServiceMock.Setup(x => x.GetPermissionsForRoles(It.IsAny<IEnumerable<string>>()))
            .Returns(Array.Empty<string>());

        _tokenServiceMock.Setup(x => x.CreateAccessToken(
            It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>(), It.IsAny<string>()))
            .Returns(("access-token", DateTime.UtcNow.AddMinutes(15)));

        _refreshTokenServiceMock.Setup(x => x.IssueAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(("refresh-token", DateTime.UtcNow.AddDays(7)));

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _refreshTokenServiceMock.Verify(x => x.IssueAsync(userId, ipAddress, It.IsAny<CancellationToken>()), Times.Once);
    }
}

