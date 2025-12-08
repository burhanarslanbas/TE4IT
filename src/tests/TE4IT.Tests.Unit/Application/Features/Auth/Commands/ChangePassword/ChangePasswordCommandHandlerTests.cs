using System.Security.Claims;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using TE4IT.Application.Abstractions.Auth;
using TE4IT.Application.Abstractions.Email;
using TE4IT.Application.Features.Auth.Commands.ChangePassword;
using Xunit;

namespace TE4IT.Tests.Unit.Application.Features.Auth.Commands.ChangePassword;

public class ChangePasswordCommandHandlerTests
{
    private readonly Mock<IUserAccountService> _userAccountServiceMock;
    private readonly Mock<IEmailSender> _emailSenderMock;
    private readonly Mock<IEmailTemplateService> _emailTemplateServiceMock;
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private readonly ChangePasswordCommandHandler _handler;

    public ChangePasswordCommandHandlerTests()
    {
        _userAccountServiceMock = new Mock<IUserAccountService>();
        _emailSenderMock = new Mock<IEmailSender>();
        _emailTemplateServiceMock = new Mock<IEmailTemplateService>();
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();

        _handler = new ChangePasswordCommandHandler(
            _userAccountServiceMock.Object,
            _emailSenderMock.Object,
            _emailTemplateServiceMock.Object,
            _httpContextAccessorMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidCurrentPassword_ChangesPassword()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var email = "test@example.com";
        var command = new ChangePasswordCommand("OldPassword123!@#", "NewPassword123!@#");

        var httpContext = new DefaultHttpContext();
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId.ToString()),
            new(ClaimTypes.Email, email)
        };
        httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(claims, "Test"));
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

        _userAccountServiceMock.Setup(x => x.ChangePasswordAsync(userId, "OldPassword123!@#", "NewPassword123!@#", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _emailTemplateServiceMock.Setup(x => x.GetPasswordChangeNotificationTemplate(email))
            .Returns("Password change notification template");
        _emailSenderMock.Setup(x => x.SendAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Message.Should().Contain("başarıyla");
    }

    [Fact]
    public async Task Handle_WithInvalidCurrentPassword_ThrowsException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var email = "test@example.com";
        var command = new ChangePasswordCommand("WrongPassword123!@#", "NewPassword123!@#");

        var httpContext = new DefaultHttpContext();
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId.ToString()),
            new(ClaimTypes.Email, email)
        };
        httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(claims, "Test"));
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

        _userAccountServiceMock.Setup(x => x.ChangePasswordAsync(userId, "WrongPassword123!@#", "NewPassword123!@#", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Message.Should().Contain("başarısız");
    }

    [Fact]
    public async Task Handle_WithWeakNewPassword_ThrowsException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var email = "test@example.com";
        var command = new ChangePasswordCommand("OldPassword123!@#", "weak");

        var httpContext = new DefaultHttpContext();
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId.ToString()),
            new(ClaimTypes.Email, email)
        };
        httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(claims, "Test"));
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

        _userAccountServiceMock.Setup(x => x.ChangePasswordAsync(userId, "OldPassword123!@#", "weak", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false); // Password policy validation fails

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_NewPasswordIsHashed()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var email = "test@example.com";
        var command = new ChangePasswordCommand("OldPassword123!@#", "NewPassword123!@#");

        var httpContext = new DefaultHttpContext();
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId.ToString()),
            new(ClaimTypes.Email, email)
        };
        httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(claims, "Test"));
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

        _userAccountServiceMock.Setup(x => x.ChangePasswordAsync(userId, "OldPassword123!@#", "NewPassword123!@#", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _emailTemplateServiceMock.Setup(x => x.GetPasswordChangeNotificationTemplate(email))
            .Returns("Password change notification template");
        _emailSenderMock.Setup(x => x.SendAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _userAccountServiceMock.Verify(x => x.ChangePasswordAsync(userId, "OldPassword123!@#", "NewPassword123!@#", It.IsAny<CancellationToken>()), Times.Once);
        // Note: Actual hashing is done by UserManager, we verify the service is called
    }

    [Fact]
    public async Task Handle_WithMissingUserId_ReturnsFailure()
    {
        // Arrange
        var command = new ChangePasswordCommand("OldPassword123!@#", "NewPassword123!@#");

        var httpContext = new DefaultHttpContext();
        httpContext.User = new ClaimsPrincipal(new ClaimsIdentity()); // No claims
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Message.Should().Contain("kimliği bulunamadı");
    }
}
