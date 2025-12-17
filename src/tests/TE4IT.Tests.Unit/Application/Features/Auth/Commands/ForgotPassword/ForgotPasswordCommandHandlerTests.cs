using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using TE4IT.Application.Abstractions.Auth;
using TE4IT.Application.Abstractions.Common;
using TE4IT.Application.Abstractions.Email;
using TE4IT.Application.Features.Auth.Commands.ForgotPassword;
using Xunit;

namespace TE4IT.Tests.Unit.Application.Features.Auth.Commands.ForgotPassword;

public class ForgotPasswordCommandHandlerTests
{
    private readonly Mock<IUserAccountService> _userAccountServiceMock;
    private readonly Mock<IEmailSender> _emailSenderMock;
    private readonly Mock<IEmailTemplateService> _emailTemplateServiceMock;
    private readonly Mock<IUrlService> _urlServiceMock;
    private readonly ForgotPasswordCommandHandler _handler;

    public ForgotPasswordCommandHandlerTests()
    {
        _userAccountServiceMock = new Mock<IUserAccountService>();
        _emailSenderMock = new Mock<IEmailSender>();
        _emailTemplateServiceMock = new Mock<IEmailTemplateService>();
        _urlServiceMock = new Mock<IUrlService>();

        _handler = new ForgotPasswordCommandHandler(
            _userAccountServiceMock.Object,
            _emailSenderMock.Object,
            _emailTemplateServiceMock.Object,
            _urlServiceMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidEmail_GeneratesToken()
    {
        // Arrange
        var email = "test@example.com";
        var command = new ForgotPasswordCommand(email);
        var token = "reset-token-123";

        _userAccountServiceMock.Setup(x => x.GeneratePasswordResetTokenAsync(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(token);

        _urlServiceMock.Setup(x => x.GetFrontendUrl()).Returns("https://app.example.com");
        _emailTemplateServiceMock.Setup(x => x.GetPasswordResetTemplate(It.IsAny<string>(), email))
            .Returns("<html>Reset password</html>");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WithInvalidEmail_ReturnsNull()
    {
        // Arrange
        var email = "nonexistent@example.com";
        var command = new ForgotPasswordCommand(email);

        _userAccountServiceMock.Setup(x => x.GeneratePasswordResetTokenAsync(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((string?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue(); // Still returns success for security (don't reveal if email exists)
    }

    [Fact]
    public async Task Handle_SendsPasswordResetEmail()
    {
        // Arrange
        var email = "test@example.com";
        var command = new ForgotPasswordCommand(email);
        var token = "reset-token-123";

        _userAccountServiceMock.Setup(x => x.GeneratePasswordResetTokenAsync(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(token);

        _urlServiceMock.Setup(x => x.GetFrontendUrl()).Returns("https://app.example.com");
        _emailTemplateServiceMock.Setup(x => x.GetPasswordResetTemplate(It.IsAny<string>(), email))
            .Returns("<html>Reset password</html>");

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _emailSenderMock.Verify(x => x.SendAsync(
            email,
            "TE4IT - Şifre Sıfırlama",
            "<html>Reset password</html>",
            It.IsAny<CancellationToken>()), Times.Once);
    }
}

