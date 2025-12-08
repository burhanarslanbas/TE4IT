using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using TE4IT.Application.Abstractions.Auth;
using TE4IT.Application.Features.Auth.Commands.RefreshToken;
using TE4IT.Domain.Constants;
using Xunit;

namespace TE4IT.Tests.Unit.Application.Features.Auth.Commands.RefreshToken;

public class RefreshTokenCommandHandlerTests
{
    private readonly Mock<IRefreshTokenService> _refreshTokenServiceMock;
    private readonly Mock<ITokenService> _tokenServiceMock;
    private readonly Mock<IUserInfoService> _userInfoServiceMock;
    private readonly Mock<IRolePermissionService> _rolePermissionServiceMock;
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private readonly RefreshTokenCommandHandler _handler;

    public RefreshTokenCommandHandlerTests()
    {
        _refreshTokenServiceMock = new Mock<IRefreshTokenService>();
        _tokenServiceMock = new Mock<ITokenService>();
        _userInfoServiceMock = new Mock<IUserInfoService>();
        _rolePermissionServiceMock = new Mock<IRolePermissionService>();
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();

        var httpContext = new DefaultHttpContext();
        httpContext.Connection.RemoteIpAddress = System.Net.IPAddress.Parse("192.168.1.1");
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

        _handler = new RefreshTokenCommandHandler(
            _refreshTokenServiceMock.Object,
            _tokenServiceMock.Object,
            _userInfoServiceMock.Object,
            _rolePermissionServiceMock.Object,
            _httpContextAccessorMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidRefreshToken_ReturnsNewTokens()
    {
        // Arrange
        var refreshToken = "valid-refresh-token";
        var userId = Guid.NewGuid();
        var command = new RefreshTokenCommand(refreshToken);

        _refreshTokenServiceMock.Setup(x => x.RefreshAsync(refreshToken, "192.168.1.1", It.IsAny<CancellationToken>()))
            .ReturnsAsync((userId, "old-access-token", DateTime.UtcNow, "new-refresh-token", DateTime.UtcNow.AddDays(7)));

        var userInfo = new UserInfo(userId, "testuser", "test@example.com", new[] { RoleNames.Administrator }, "v1.0");
        _userInfoServiceMock.Setup(x => x.GetUserInfoAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(userInfo);

        _rolePermissionServiceMock.Setup(x => x.GetPermissionsForRoles(It.IsAny<IEnumerable<string>>()))
            .Returns(new[] { Permissions.Project.Create });

        _tokenServiceMock.Setup(x => x.CreateAccessToken(
            userId, "testuser", "test@example.com", It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>(), "v1.0"))
            .Returns(("new-access-token", DateTime.UtcNow.AddMinutes(15)));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.AccessToken.Should().Be("new-access-token");
        result.RefreshToken.Should().Be("new-refresh-token");
    }

    [Fact]
    public async Task Handle_WithExpiredToken_ThrowsException()
    {
        // Arrange
        var refreshToken = "expired-refresh-token";
        var command = new RefreshTokenCommand(refreshToken);

        _refreshTokenServiceMock.Setup(x => x.RefreshAsync(refreshToken, "192.168.1.1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(default((Guid, string, DateTime, string, DateTime)?));

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact]
    public async Task Handle_WithRevokedToken_ThrowsException()
    {
        // Arrange
        var refreshToken = "revoked-refresh-token";
        var command = new RefreshTokenCommand(refreshToken);

        _refreshTokenServiceMock.Setup(x => x.RefreshAsync(refreshToken, "192.168.1.1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(default((Guid, string, DateTime, string, DateTime)?));

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact]
    public async Task Handle_RotatesRefreshToken()
    {
        // Arrange
        var oldRefreshToken = "old-refresh-token";
        var userId = Guid.NewGuid();
        var command = new RefreshTokenCommand(oldRefreshToken);

        _refreshTokenServiceMock.Setup(x => x.RefreshAsync(oldRefreshToken, "192.168.1.1", It.IsAny<CancellationToken>()))
            .ReturnsAsync((userId, "old-access-token", DateTime.UtcNow, "new-refresh-token", DateTime.UtcNow.AddDays(7)));

        var userInfo = new UserInfo(userId, "testuser", "test@example.com", Array.Empty<string>(), null);
        _userInfoServiceMock.Setup(x => x.GetUserInfoAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(userInfo);

        _rolePermissionServiceMock.Setup(x => x.GetPermissionsForRoles(It.IsAny<IEnumerable<string>>()))
            .Returns(Array.Empty<string>());

        _tokenServiceMock.Setup(x => x.CreateAccessToken(
            It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>(), It.IsAny<string>()))
            .Returns(("new-access-token", DateTime.UtcNow.AddMinutes(15)));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.RefreshToken.Should().NotBe(oldRefreshToken);
        result.RefreshToken.Should().Be("new-refresh-token");
    }

    [Fact]
    public async Task Handle_RevokesOldToken()
    {
        // Arrange
        var oldRefreshToken = "old-refresh-token";
        var userId = Guid.NewGuid();
        var command = new RefreshTokenCommand(oldRefreshToken);

        _refreshTokenServiceMock.Setup(x => x.RefreshAsync(oldRefreshToken, "192.168.1.1", It.IsAny<CancellationToken>()))
            .ReturnsAsync((userId, "old-access-token", DateTime.UtcNow, "new-refresh-token", DateTime.UtcNow.AddDays(7)));

        var userInfo = new UserInfo(userId, "testuser", "test@example.com", Array.Empty<string>(), null);
        _userInfoServiceMock.Setup(x => x.GetUserInfoAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(userInfo);

        _rolePermissionServiceMock.Setup(x => x.GetPermissionsForRoles(It.IsAny<IEnumerable<string>>()))
            .Returns(Array.Empty<string>());

        _tokenServiceMock.Setup(x => x.CreateAccessToken(
            It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>(), It.IsAny<string>()))
            .Returns(("new-access-token", DateTime.UtcNow.AddMinutes(15)));

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _refreshTokenServiceMock.Verify(x => x.RefreshAsync(oldRefreshToken, "192.168.1.1", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithInvalidToken_ThrowsException()
    {
        // Arrange
        var invalidToken = "invalid-token";
        var command = new RefreshTokenCommand(invalidToken);

        _refreshTokenServiceMock.Setup(x => x.RefreshAsync(invalidToken, "192.168.1.1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(default((Guid, string, DateTime, string, DateTime)?));

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact]
    public async Task Handle_RecordsIpAddress()
    {
        // Arrange
        var refreshToken = "valid-refresh-token";
        var userId = Guid.NewGuid();
        var ipAddress = "192.168.1.100";
        var command = new RefreshTokenCommand(refreshToken);

        var httpContext = new DefaultHttpContext();
        httpContext.Connection.RemoteIpAddress = System.Net.IPAddress.Parse(ipAddress);
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

        _refreshTokenServiceMock.Setup(x => x.RefreshAsync(refreshToken, ipAddress, It.IsAny<CancellationToken>()))
            .ReturnsAsync((userId, "old-access-token", DateTime.UtcNow, "new-refresh-token", DateTime.UtcNow.AddDays(7)));

        var userInfo = new UserInfo(userId, "testuser", "test@example.com", Array.Empty<string>(), null);
        _userInfoServiceMock.Setup(x => x.GetUserInfoAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(userInfo);

        _rolePermissionServiceMock.Setup(x => x.GetPermissionsForRoles(It.IsAny<IEnumerable<string>>()))
            .Returns(Array.Empty<string>());

        _tokenServiceMock.Setup(x => x.CreateAccessToken(
            It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>(), It.IsAny<string>()))
            .Returns(("new-access-token", DateTime.UtcNow.AddMinutes(15)));

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _refreshTokenServiceMock.Verify(x => x.RefreshAsync(refreshToken, ipAddress, It.IsAny<CancellationToken>()), Times.Once);
    }
}

