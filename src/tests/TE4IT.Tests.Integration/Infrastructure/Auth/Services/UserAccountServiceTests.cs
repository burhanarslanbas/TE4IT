using FluentAssertions;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using TE4IT.Domain.Constants;
using TE4IT.Domain.Exceptions.Auth;
using TE4IT.Persistence.Common.Contexts;
using TE4IT.Persistence.Common.Identity;
using TE4IT.Tests.Integration.Common;
using Xunit;

namespace TE4IT.Tests.Integration.Infrastructure.Auth.Services;

public class UserAccountServiceTests : IntegrationTestBase
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly TE4IT.Infrastructure.Auth.Services.UserAccountService _service;

    public UserAccountServiceTests()
    {
        var userStore = new UserStore<AppUser, IdentityRole<Guid>, AppDbContext, Guid>(DbContext);
        var passwordHasher = new PasswordHasher<AppUser>();
        var userValidators = new List<IUserValidator<AppUser>>();
        // Add default password validators to enforce password policy
        var passwordValidators = new List<IPasswordValidator<AppUser>>
        {
            new PasswordValidator<AppUser>()
        };
        var lookupNormalizer = new UpperInvariantLookupNormalizer();
        var errors = new IdentityErrorDescriber();
        var services = new ServiceCollection();
        services.AddLogging();
        var serviceProvider = services.BuildServiceProvider();
        var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
        var userLogger = loggerFactory.CreateLogger<UserManager<AppUser>>();
        var signInLogger = loggerFactory.CreateLogger<SignInManager<AppUser>>();
        
        // Add IHttpContextAccessor mock for SignInManager
        var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        httpContextAccessorMock.Setup(x => x.HttpContext).Returns(new DefaultHttpContext());
        services.AddSingleton(httpContextAccessorMock.Object);

        var identityOptions = Options.Create(new IdentityOptions
        {
            Password = new PasswordOptions
            {
                RequireDigit = true,
                RequiredLength = 8,
                RequireNonAlphanumeric = true,
                RequireUppercase = true,
                RequireLowercase = true,
                RequiredUniqueChars = 3
            },
            Lockout = new LockoutOptions
            {
                AllowedForNewUsers = true,
                MaxFailedAccessAttempts = 5,
                DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15)
            }
        });

        _userManager = new UserManager<AppUser>(
            userStore,
            identityOptions,
            passwordHasher,
            userValidators,
            passwordValidators,
            lookupNormalizer,
            errors,
            serviceProvider,
            userLogger);
        
        // Add default token providers for password reset
        var dataProtectionProvider = DataProtectionProvider.Create("Test");
        var tokenProviderOptions = Options.Create(new DataProtectionTokenProviderOptions());
        var tokenProviderLogger = loggerFactory.CreateLogger<DataProtectorTokenProvider<AppUser>>();
        var tokenProvider = new DataProtectorTokenProvider<AppUser>(
            dataProtectionProvider,
            tokenProviderOptions,
            tokenProviderLogger);
        _userManager.RegisterTokenProvider(TokenOptions.DefaultProvider, tokenProvider);

        serviceProvider = services.BuildServiceProvider();
        
        _signInManager = new SignInManager<AppUser>(
            _userManager,
            serviceProvider.GetRequiredService<IHttpContextAccessor>(),
            new Mock<IUserClaimsPrincipalFactory<AppUser>>().Object,
            identityOptions,
            signInLogger,
            new Mock<Microsoft.AspNetCore.Authentication.IAuthenticationSchemeProvider>().Object,
            new Mock<IUserConfirmation<AppUser>>().Object);

        _service = new TE4IT.Infrastructure.Auth.Services.UserAccountService(_userManager, _signInManager);
    }

    [Fact]
    public async Task RegisterAsync_WithValidData_CreatesUser()
    {
        // Arrange
        var userName = "testuser";
        var email = "test@example.com";
        var password = "Test123!@#";

        // Act
        var userId = await _service.RegisterAsync(userName, email, password, CancellationToken.None);

        // Assert
        userId.Should().NotBeEmpty();
        var user = await _userManager.FindByIdAsync(userId.ToString());
        user.Should().NotBeNull();
        user!.UserName.Should().Be(userName);
        user.Email.Should().Be(email);
    }

    [Fact]
    public async Task RegisterAsync_WithDuplicateEmail_ThrowsException()
    {
        // Arrange
        var email = "duplicate@example.com";
        await _service.RegisterAsync("user1", email, "Test123!@#", CancellationToken.None);

        // Act
        var act = async () => await _service.RegisterAsync("user2", email, "Test123!@#", CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<DuplicateEmailException>();
    }

    [Fact]
    public async Task RegisterAsync_WithDuplicateUserName_ThrowsException()
    {
        // Arrange
        var userName = "duplicateuser";
        await _service.RegisterAsync(userName, "email1@example.com", "Test123!@#", CancellationToken.None);

        // Act
        var act = async () => await _service.RegisterAsync(userName, "email2@example.com", "Test123!@#", CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<DuplicateUserNameException>();
    }

    [Fact]
    public async Task RegisterAsync_WithWeakPassword_ThrowsException()
    {
        // Arrange
        var weakPassword = "weak";

        // Act
        var act = async () => await _service.RegisterAsync("testuser", "test@example.com", weakPassword, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<UserRegistrationFailedException>();
    }

    [Fact]
    public async Task RegisterAsync_PasswordIsHashed()
    {
        // Arrange
        var password = "Test123!@#";
        var userId = await _service.RegisterAsync("testuser", "test@example.com", password, CancellationToken.None);

        // Assert
        var user = await _userManager.FindByIdAsync(userId.ToString());
        user.Should().NotBeNull();
        user!.PasswordHash.Should().NotBeNullOrEmpty();
        user.PasswordHash.Should().NotBe(password);
    }

    [Fact]
    public async Task RegisterAsync_AssignsTrialRole()
    {
        // Arrange
        var userId = await _service.RegisterAsync("testuser", "test@example.com", "Test123!@#", CancellationToken.None);

        // Act
        var roles = await _userManager.GetRolesAsync(await _userManager.FindByIdAsync(userId.ToString())!);

        // Assert
        roles.Should().Contain(RoleNames.Trial);
    }

    [Fact]
    public async Task ValidateCredentialsAsync_WithValidCredentials_ReturnsUserId()
    {
        // Arrange
        var email = "test@example.com";
        var password = "Test123!@#";
        var userId = await _service.RegisterAsync("testuser", email, password, CancellationToken.None);

        // Act
        var result = await _service.ValidateCredentialsAsync(email, password, CancellationToken.None);

        // Assert
        result.Should().Be(userId);
    }

    [Fact]
    public async Task ValidateCredentialsAsync_WithInvalidPassword_ThrowsException()
    {
        // Arrange
        var email = "test@example.com";
        await _service.RegisterAsync("testuser", email, "Test123!@#", CancellationToken.None);

        // Act
        var act = async () => await _service.ValidateCredentialsAsync(email, "WrongPassword123!@#", CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidCredentialsException>();
    }

    [Fact]
    public async Task ValidateCredentialsAsync_WithNonExistentUser_ThrowsException()
    {
        // Act
        var act = async () => await _service.ValidateCredentialsAsync("nonexistent@example.com", "Test123!@#", CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidCredentialsException>();
    }

    [Fact]
    public async Task ValidateCredentialsAsync_WithLockedAccount_ThrowsException()
    {
        // Arrange
        var email = "test@example.com";
        var password = "Test123!@#";
        await _service.RegisterAsync("testuser", email, password, CancellationToken.None);

        // Lock out the account
        var user = await _userManager.FindByEmailAsync(email);
        await _userManager.SetLockoutEnabledAsync(user!, true);
        await _userManager.SetLockoutEndDateAsync(user!, DateTimeOffset.UtcNow.AddMinutes(15));

        // Act
        var act = async () => await _service.ValidateCredentialsAsync(email, password, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidCredentialsException>();
    }

    [Fact]
    public async Task GeneratePasswordResetTokenAsync_WithValidEmail_ReturnsToken()
    {
        // Arrange
        var email = "test@example.com";
        await _service.RegisterAsync("testuser", email, "Test123!@#", CancellationToken.None);

        // Act
        var token = await _service.GeneratePasswordResetTokenAsync(email, CancellationToken.None);

        // Assert
        token.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task GeneratePasswordResetTokenAsync_WithInvalidEmail_ReturnsNull()
    {
        // Act
        var token = await _service.GeneratePasswordResetTokenAsync("nonexistent@example.com", CancellationToken.None);

        // Assert
        token.Should().BeNull();
    }

    [Fact]
    public async Task ResetPasswordAsync_WithValidToken_ReturnsTrue()
    {
        // Arrange
        var email = "test@example.com";
        var oldPassword = "Test123!@#";
        var newPassword = "NewPassword123!@#";
        await _service.RegisterAsync("testuser", email, oldPassword, CancellationToken.None);
        var token = await _service.GeneratePasswordResetTokenAsync(email, CancellationToken.None);

        // Act
        var result = await _service.ResetPasswordAsync(email, token!, newPassword, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
        
        // Verify new password works
        var userId = await _service.ValidateCredentialsAsync(email, newPassword, CancellationToken.None);
        userId.Should().NotBeEmpty();
    }

    [Fact]
    public async Task ResetPasswordAsync_WithInvalidToken_ReturnsFalse()
    {
        // Arrange
        var email = "test@example.com";
        await _service.RegisterAsync("testuser", email, "Test123!@#", CancellationToken.None);

        // Act
        var result = await _service.ResetPasswordAsync(email, "invalid-token", "NewPassword123!@#", CancellationToken.None);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task ChangePasswordAsync_WithValidPassword_ReturnsTrue()
    {
        // Arrange
        var email = "test@example.com";
        var currentPassword = "Test123!@#";
        var newPassword = "NewPassword123!@#";
        var userId = await _service.RegisterAsync("testuser", email, currentPassword, CancellationToken.None);

        // Act
        var result = await _service.ChangePasswordAsync(userId, currentPassword, newPassword, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
        
        // Verify new password works
        var validatedUserId = await _service.ValidateCredentialsAsync(email, newPassword, CancellationToken.None);
        validatedUserId.Should().Be(userId);
    }

    [Fact]
    public async Task ChangePasswordAsync_WithInvalidCurrentPassword_ReturnsFalse()
    {
        // Arrange
        var userId = await _service.RegisterAsync("testuser", "test@example.com", "Test123!@#", CancellationToken.None);

        // Act
        var result = await _service.ChangePasswordAsync(userId, "WrongPassword123!@#", "NewPassword123!@#", CancellationToken.None);

        // Assert
        result.Should().BeFalse();
    }
}

