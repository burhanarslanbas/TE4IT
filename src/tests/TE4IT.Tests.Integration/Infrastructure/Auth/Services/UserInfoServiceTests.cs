using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TE4IT.Domain.Constants;
using TE4IT.Persistence.Common.Contexts;
using TE4IT.Persistence.Common.Identity;
using TE4IT.Tests.Integration.Common;
using Xunit;

namespace TE4IT.Tests.Integration.Infrastructure.Auth.Services;

public class UserInfoServiceTests : IntegrationTestBase
{
    private readonly UserManager<AppUser> _userManager;
    private readonly TE4IT.Infrastructure.Auth.Services.UserInfoService _service;

    public UserInfoServiceTests()
    {
        var userStore = new UserStore<AppUser, IdentityRole<Guid>, AppDbContext, Guid>(DbContext);
        var passwordHasher = new PasswordHasher<AppUser>();
        var userValidators = new List<IUserValidator<AppUser>>();
        var passwordValidators = new List<IPasswordValidator<AppUser>>();
        var lookupNormalizer = new UpperInvariantLookupNormalizer();
        var errors = new IdentityErrorDescriber();
        var services = new ServiceCollection();
        services.AddLogging();
        var serviceProvider = services.BuildServiceProvider();
        var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
        var userLogger = loggerFactory.CreateLogger<UserManager<AppUser>>();

        _userManager = new UserManager<AppUser>(
            userStore,
            Options.Create(new IdentityOptions()),
            passwordHasher,
            userValidators,
            passwordValidators,
            lookupNormalizer,
            errors,
            serviceProvider,
            userLogger);

        _service = new TE4IT.Infrastructure.Auth.Services.UserInfoService(_userManager);
    }

    [Fact]
    public async Task GetUserInfoAsync_WithValidUserId_ReturnsUserInfo()
    {
        // Arrange
        var user = new AppUser
        {
            Id = Guid.NewGuid(),
            UserName = "testuser",
            Email = "test@example.com",
            NormalizedUserName = "TESTUSER",
            NormalizedEmail = "TEST@EXAMPLE.COM"
        };
        await _userManager.CreateAsync(user, "Test123!@#");
        await _userManager.AddToRoleAsync(user, RoleNames.Administrator);
        await DbContext.SaveChangesAsync();

        // Act
        var result = await _service.GetUserInfoAsync(user.Id, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(user.Id);
        result.UserName.Should().Be("testuser");
        result.Email.Should().Be("test@example.com");
    }

    [Fact]
    public async Task GetUserInfoAsync_WithInvalidUserId_ReturnsNull()
    {
        // Arrange
        var invalidUserId = Guid.NewGuid();

        // Act
        var result = await _service.GetUserInfoAsync(invalidUserId, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetUserInfoAsync_ReturnsRoles()
    {
        // Arrange
        var user = new AppUser
        {
            Id = Guid.NewGuid(),
            UserName = "testuser",
            Email = "test@example.com",
            NormalizedUserName = "TESTUSER",
            NormalizedEmail = "TEST@EXAMPLE.COM"
        };
        await _userManager.CreateAsync(user, "Test123!@#");
        await _userManager.AddToRoleAsync(user, RoleNames.Administrator);
        await _userManager.AddToRoleAsync(user, RoleNames.TeamLead);
        await DbContext.SaveChangesAsync();

        // Act
        var result = await _service.GetUserInfoAsync(user.Id, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Roles.Should().Contain(RoleNames.Administrator);
        result.Roles.Should().Contain(RoleNames.TeamLead);
    }

    [Fact]
    public async Task GetUserInfoAsync_ReturnsPermissionsVersion()
    {
        // Arrange
        var user = new AppUser
        {
            Id = Guid.NewGuid(),
            UserName = "testuser",
            Email = "test@example.com",
            NormalizedUserName = "TESTUSER",
            NormalizedEmail = "TEST@EXAMPLE.COM"
        };
        await _userManager.CreateAsync(user, "Test123!@#");
        await DbContext.SaveChangesAsync();

        // Act
        var result = await _service.GetUserInfoAsync(user.Id, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.PermissionsVersion.Should().NotBeNullOrEmpty(); // SecurityStamp
    }

    [Fact]
    public async Task GetUserInfoAsync_ReturnsEmail()
    {
        // Arrange
        var email = "test@example.com";
        var user = new AppUser
        {
            Id = Guid.NewGuid(),
            UserName = "testuser",
            Email = email,
            NormalizedUserName = "TESTUSER",
            NormalizedEmail = "TEST@EXAMPLE.COM"
        };
        await _userManager.CreateAsync(user, "Test123!@#");
        await DbContext.SaveChangesAsync();

        // Act
        var result = await _service.GetUserInfoAsync(user.Id, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Email.Should().Be(email);
    }

    [Fact]
    public async Task GetUserInfoAsync_ReturnsUserName()
    {
        // Arrange
        var userName = "testuser";
        var user = new AppUser
        {
            Id = Guid.NewGuid(),
            UserName = userName,
            Email = "test@example.com",
            NormalizedUserName = "TESTUSER",
            NormalizedEmail = "TEST@EXAMPLE.COM"
        };
        await _userManager.CreateAsync(user, "Test123!@#");
        await DbContext.SaveChangesAsync();

        // Act
        var result = await _service.GetUserInfoAsync(user.Id, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.UserName.Should().Be(userName);
    }
}

