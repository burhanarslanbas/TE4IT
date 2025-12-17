using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using TE4IT.Application.Abstractions.Auth;
using TE4IT.Application.Abstractions.Email;
using TE4IT.Application.Features.Auth.Commands.ResetPassword;
using Xunit;

namespace TE4IT.Tests.Unit.Application.Features.Auth.Commands.ResetPassword;

public class ResetPasswordCommandHandlerTests
{
    private readonly Mock<IUserAccountService> _userAccountServiceMock;
    private readonly Mock<IEmailSender> _emailSenderMock;
    private readonly Mock<IEmailTemplateService> _emailTemplateServiceMock;
    private readonly ResetPasswordCommandHandler _handler;

    public ResetPasswordCommandHandlerTests()
    {
        _userAccountServiceMock = new Mock<IUserAccountService>();
        _emailSenderMock = new Mock<IEmailSender>();
        _emailTemplateServiceMock = new Mock<IEmailTemplateService>();
        
        _handler = new ResetPasswordCommandHandler(
            _userAccountServiceMock.Object,
            _emailSenderMock.Object,
            _emailTemplateServiceMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidToken_ResetsPassword()
    {
        // Arrange
        var email = "test@example.com";
        var token = "valid-reset-token";
        var newPassword = "NewPassword123!@#";
        var command = new ResetPasswordCommand(email, token, newPassword);

        _userAccountServiceMock.Setup(x => x.ResetPasswordAsync(email, token, newPassword, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WithInvalidToken_ReturnsFalse()
    {
        // Arrange
        var email = "test@example.com";
        var token = "invalid-token";
        var newPassword = "NewPassword123!@#";
        var command = new ResetPasswordCommand(email, token, newPassword);

        _userAccountServiceMock.Setup(x => x.ResetPasswordAsync(email, token, newPassword, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_WithExpiredToken_ReturnsFalse()
    {
        // Arrange
        var email = "test@example.com";
        var token = "expired-token";
        var newPassword = "NewPassword123!@#";
        var command = new ResetPasswordCommand(email, token, newPassword);

        _userAccountServiceMock.Setup(x => x.ResetPasswordAsync(email, token, newPassword, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
    }
}

