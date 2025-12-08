using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using TE4IT.Application.Abstractions.Auth;
using TE4IT.Application.Abstractions.Common;
using TE4IT.Application.Abstractions.Email;
using TE4IT.Application.Features.Auth.Commands.Register;
using TE4IT.Domain.Constants;
using TE4IT.Domain.Exceptions.Auth;
using Xunit;

namespace TE4IT.Tests.Unit.Application.Features.Auth.Commands.Register;

public class RegisterCommandHandlerTests
{
    private readonly Mock<IUserAccountService> _userAccountServiceMock;
    private readonly Mock<IUserInfoService> _userInfoServiceMock;
    private readonly Mock<IRolePermissionService> _rolePermissionServiceMock;
    private readonly Mock<ITokenService> _tokenServiceMock;
    private readonly Mock<IRefreshTokenService> _refreshTokenServiceMock;
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private readonly Mock<IEmailSender> _emailSenderMock;
    private readonly Mock<IEmailTemplateService> _emailTemplateServiceMock;
    private readonly Mock<IUrlService> _urlServiceMock;
    private readonly RegisterCommandHandler _handler;

    public RegisterCommandHandlerTests()
    {
        _userAccountServiceMock = new Mock<IUserAccountService>();
        _userInfoServiceMock = new Mock<IUserInfoService>();
        _rolePermissionServiceMock = new Mock<IRolePermissionService>();
        _tokenServiceMock = new Mock<ITokenService>();
        _refreshTokenServiceMock = new Mock<IRefreshTokenService>();
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        _emailSenderMock = new Mock<IEmailSender>();
        _emailTemplateServiceMock = new Mock<IEmailTemplateService>();
        _urlServiceMock = new Mock<IUrlService>();

        var httpContext = new DefaultHttpContext();
        httpContext.Connection.RemoteIpAddress = System.Net.IPAddress.Parse("192.168.1.1");
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

        _handler = new RegisterCommandHandler(
            _userAccountServiceMock.Object,
            _userInfoServiceMock.Object,
            _rolePermissionServiceMock.Object,
            _tokenServiceMock.Object,
            _refreshTokenServiceMock.Object,
            _httpContextAccessorMock.Object,
            _emailSenderMock.Object,
            _emailTemplateServiceMock.Object,
            _urlServiceMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidData_CreatesUserAndReturnsTokens()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var userName = "testuser";
        var email = "test@example.com";
        var password = "Test123!@#";
        var command = new RegisterCommand(userName, email, password);

        _userAccountServiceMock.Setup(x => x.RegisterAsync(userName, email, password, It.IsAny<CancellationToken>()))
            .ReturnsAsync(userId);

        var userInfo = new UserInfo(userId, userName, email, new[] { RoleNames.Trial }, "v1.0");
        _userInfoServiceMock.Setup(x => x.GetUserInfoAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(userInfo);

        _rolePermissionServiceMock.Setup(x => x.GetPermissionsForRoles(It.IsAny<IEnumerable<string>>()))
            .Returns(new[] { Permissions.Project.Create, Permissions.Project.View });

        _tokenServiceMock.Setup(x => x.CreateAccessToken(
            userId, userName, email, It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>(), "v1.0"))
            .Returns(("access-token", DateTime.UtcNow.AddMinutes(15)));

        _refreshTokenServiceMock.Setup(x => x.IssueAsync(userId, "192.168.1.1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(("refresh-token", DateTime.UtcNow.AddDays(7)));

        _urlServiceMock.Setup(x => x.GetFrontendUrl()).Returns("https://app.example.com");
        _emailTemplateServiceMock.Setup(x => x.GetWelcomeTemplate(userName, email, "https://app.example.com"))
            .Returns("Welcome email template");
        _emailSenderMock.Setup(x => x.SendAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.UserId.Should().Be(userId);
        result.UserName.Should().Be(userName);
        result.Email.Should().Be(email);
        result.AccessToken.Should().Be("access-token");
        result.RefreshToken.Should().Be("refresh-token");
    }

    [Fact]
    public async Task Handle_WithDuplicateEmail_ThrowsException()
    {
        // Arrange
        var command = new RegisterCommand("testuser", "test@example.com", "Test123!@#");

        _userAccountServiceMock.Setup(x => x.RegisterAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new DuplicateEmailException("test@example.com"));

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<DuplicateEmailException>();
    }

    [Fact]
    public async Task Handle_SendsWelcomeEmail()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var userName = "testuser";
        var email = "test@example.com";
        var command = new RegisterCommand(userName, email, "Test123!@#");

        _userAccountServiceMock.Setup(x => x.RegisterAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(userId);

        var userInfo = new UserInfo(userId, userName, email, Array.Empty<string>(), null);
        _userInfoServiceMock.Setup(x => x.GetUserInfoAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(userInfo);

        _rolePermissionServiceMock.Setup(x => x.GetPermissionsForRoles(It.IsAny<IEnumerable<string>>()))
            .Returns(Array.Empty<string>());

        _tokenServiceMock.Setup(x => x.CreateAccessToken(
            It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>(), It.IsAny<string>()))
            .Returns(("access-token", DateTime.UtcNow.AddMinutes(15)));

        _refreshTokenServiceMock.Setup(x => x.IssueAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(("refresh-token", DateTime.UtcNow.AddDays(7)));

        _urlServiceMock.Setup(x => x.GetFrontendUrl()).Returns("https://app.example.com");
        _emailTemplateServiceMock.Setup(x => x.GetWelcomeTemplate(userName, email, "https://app.example.com"))
            .Returns("Welcome email template");
        _emailSenderMock.Setup(x => x.SendAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Wait a bit for background task
        await Task.Delay(100);

        // Assert
        _emailTemplateServiceMock.Verify(x => x.GetWelcomeTemplate(userName, email, "https://app.example.com"), Times.Once);
        // Note: Email sending is in background task, so we verify template was called
    }

    [Fact]
    public async Task Handle_EmailFailure_DoesNotBlockRegistration()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new RegisterCommand("testuser", "test@example.com", "Test123!@#");

        _userAccountServiceMock.Setup(x => x.RegisterAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
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

        _urlServiceMock.Setup(x => x.GetFrontendUrl()).Returns("https://app.example.com");
        _emailTemplateServiceMock.Setup(x => x.GetWelcomeTemplate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns("Welcome email template");
        _emailSenderMock.Setup(x => x.SendAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Email service unavailable"));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert - Registration should succeed even if email fails
        result.Should().NotBeNull();
        result!.UserId.Should().Be(userId);
    }

    [Fact]
    public async Task Handle_AssignsTrialRole()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new RegisterCommand("testuser", "test@example.com", "Test123!@#");

        _userAccountServiceMock.Setup(x => x.RegisterAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(userId);

        var userInfo = new UserInfo(userId, "testuser", "test@example.com", new[] { RoleNames.Trial }, null);
        _userInfoServiceMock.Setup(x => x.GetUserInfoAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(userInfo);

        _rolePermissionServiceMock.Setup(x => x.GetPermissionsForRoles(new[] { RoleNames.Trial }))
            .Returns(new[] { Permissions.Project.Create, Permissions.Project.View });

        _tokenServiceMock.Setup(x => x.CreateAccessToken(
            It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), new[] { RoleNames.Trial }, It.IsAny<IEnumerable<string>>(), It.IsAny<string>()))
            .Returns(("access-token", DateTime.UtcNow.AddMinutes(15)));

        _refreshTokenServiceMock.Setup(x => x.IssueAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(("refresh-token", DateTime.UtcNow.AddDays(7)));

        _urlServiceMock.Setup(x => x.GetFrontendUrl()).Returns("https://app.example.com");
        _emailTemplateServiceMock.Setup(x => x.GetWelcomeTemplate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns("Welcome email template");
        _emailSenderMock.Setup(x => x.SendAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _tokenServiceMock.Verify(x => x.CreateAccessToken(
            It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), new[] { RoleNames.Trial }, It.IsAny<IEnumerable<string>>(), It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithWeakPassword_ThrowsException()
    {
        // Arrange
        var command = new RegisterCommand("testuser", "test@example.com", "weak");

        _userAccountServiceMock.Setup(x => x.RegisterAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new UserRegistrationFailedException("Password does not meet requirements"));

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<UserRegistrationFailedException>();
    }
}
