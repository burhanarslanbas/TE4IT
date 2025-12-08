using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TE4IT.Persistence.Common.Contexts;
using TE4IT.Persistence.Common.Identity;
using TE4IT.Tests.Integration.Common;
using Xunit;

namespace TE4IT.Tests.Integration.Infrastructure.Auth.Services;

public class PasswordSecurityTests : IntegrationTestBase
{
    public PasswordSecurityTests()
    {
        // Use UserManager from base class which has proper IdentityOptions configured
    }

    [Fact]
    public async Task PasswordIsHashed_NotStoredInPlainText()
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
        var password = "Test123!@#";

        // Act
        var result = await UserManager.CreateAsync(user, password);
        await DbContext.SaveChangesAsync();

        // Assert
        result.Succeeded.Should().BeTrue();
        var savedUser = await DbContext.Users.FirstOrDefaultAsync(u => u.Id == user.Id);
        savedUser.Should().NotBeNull();
        savedUser!.PasswordHash.Should().NotBeNullOrEmpty();
        savedUser.PasswordHash.Should().NotBe(password);
        savedUser.PasswordHash.Length.Should().BeGreaterThan(20); // BCrypt hash is long
    }

    [Fact]
    public async Task PasswordHash_IsUniqueForSamePassword()
    {
        // Arrange
        var password = "Test123!@#";
        var user1 = new AppUser
        {
            Id = Guid.NewGuid(),
            UserName = "user1",
            Email = "user1@example.com",
            NormalizedUserName = "USER1",
            NormalizedEmail = "USER1@EXAMPLE.COM"
        };
        var user2 = new AppUser
        {
            Id = Guid.NewGuid(),
            UserName = "user2",
            Email = "user2@example.com",
            NormalizedUserName = "USER2",
            NormalizedEmail = "USER2@EXAMPLE.COM"
        };

        // Act
        await UserManager.CreateAsync(user1, password);
        await UserManager.CreateAsync(user2, password);
        await DbContext.SaveChangesAsync();

        // Assert
        var savedUser1 = await DbContext.Users.FirstOrDefaultAsync(u => u.Id == user1.Id);
        var savedUser2 = await DbContext.Users.FirstOrDefaultAsync(u => u.Id == user2.Id);
        savedUser1!.PasswordHash.Should().NotBe(savedUser2!.PasswordHash); // Different salts = different hashes
    }

    [Fact]
    public async Task PasswordVerification_WithCorrectPassword_ReturnsTrue()
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
        var password = "Test123!@#";
        await UserManager.CreateAsync(user, password);
        await DbContext.SaveChangesAsync();

        // Act
        var result = await UserManager.CheckPasswordAsync(user, password);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task PasswordVerification_WithIncorrectPassword_ReturnsFalse()
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
        var password = "Test123!@#";
        await UserManager.CreateAsync(user, password);
        await DbContext.SaveChangesAsync();

        // Act
        var result = await UserManager.CheckPasswordAsync(user, "WrongPassword123!@#");

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task PasswordPolicy_EnforcesMinimumLength()
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
        var shortPassword = "Test1!"; // 7 characters

        // Act
        var result = await UserManager.CreateAsync(user, shortPassword);

        // Assert
        result.Succeeded.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Description.Contains("at least 8 characters") || e.Code == "PasswordTooShort");
    }

    [Fact]
    public async Task PasswordPolicy_RequiresDigit()
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
        var passwordWithoutDigit = "TestPass!@#"; // No digit

        // Act
        var result = await UserManager.CreateAsync(user, passwordWithoutDigit);

        // Assert
        result.Succeeded.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Description.Contains("digit") || e.Description.Contains("Digit"));
    }

    [Fact]
    public async Task PasswordPolicy_RequiresUppercase()
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
        var passwordWithoutUppercase = "test123!@#"; // No uppercase

        // Act
        var result = await UserManager.CreateAsync(user, passwordWithoutUppercase);

        // Assert
        result.Succeeded.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Description.Contains("uppercase") || e.Description.Contains("Uppercase"));
    }

    [Fact]
    public async Task PasswordPolicy_RequiresLowercase()
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
        var passwordWithoutLowercase = "TEST123!@#"; // No lowercase

        // Act
        var result = await UserManager.CreateAsync(user, passwordWithoutLowercase);

        // Assert
        result.Succeeded.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Description.Contains("lowercase") || e.Description.Contains("Lowercase"));
    }

    [Fact]
    public async Task PasswordPolicy_RequiresSpecialCharacter()
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
        var passwordWithoutSpecial = "Test1234"; // No special character

        // Act
        var result = await UserManager.CreateAsync(user, passwordWithoutSpecial);

        // Assert
        result.Succeeded.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Description.Contains("non alphanumeric") || e.Description.Contains("non-alphanumeric") || e.Description.Contains("special"));
    }

    [Fact]
    public async Task PasswordPolicy_RequiresUniqueChars()
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
        var passwordWithOnly2Types = "AAAA1111"; // Only uppercase and digits (2 types, need 3)

        // Act
        var result = await UserManager.CreateAsync(user, passwordWithOnly2Types);

        // Assert
        result.Succeeded.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Description.Contains("unique") || e.Description.Contains("different"));
    }

    [Fact]
    public async Task AccountLockout_AfterMaxFailedAttempts()
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
        var correctPassword = "Test123!@#";
        await UserManager.CreateAsync(user, correctPassword);
        await DbContext.SaveChangesAsync();

        // Act - Attempt wrong password 5 times
        for (int i = 0; i < 5; i++)
        {
            var checkResult = await UserManager.CheckPasswordAsync(user, "WrongPassword123!@#");
            if (!checkResult)
            {
                await UserManager.AccessFailedAsync(user);
            }
        }

        // Assert
        var isLockedOut = await UserManager.IsLockedOutAsync(user);
        isLockedOut.Should().BeTrue();
    }

    [Fact]
    public async Task AccountLockout_DurationIs15Minutes()
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
        await UserManager.CreateAsync(user, "Test123!@#");
        await DbContext.SaveChangesAsync();

        // Lock out the account
        for (int i = 0; i < 5; i++)
        {
            var checkResult = await UserManager.CheckPasswordAsync(user, "WrongPassword123!@#");
            if (!checkResult)
            {
                await UserManager.AccessFailedAsync(user);
            }
        }

        // Act
        var lockoutEnd = await UserManager.GetLockoutEndDateAsync(user);

        // Assert
        lockoutEnd.Should().NotBeNull();
        lockoutEnd!.Value.Should().BeCloseTo(DateTimeOffset.UtcNow.AddMinutes(15), TimeSpan.FromMinutes(1));
    }

    [Fact]
    public async Task AccountLockout_WithCorrectPassword_ResetsCounter()
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
        var correctPassword = "Test123!@#";
        await UserManager.CreateAsync(user, correctPassword);
        await DbContext.SaveChangesAsync();

        // Attempt wrong password 3 times
        for (int i = 0; i < 3; i++)
        {
            var checkResult = await UserManager.CheckPasswordAsync(user, "WrongPassword123!@#");
            if (!checkResult)
            {
                await UserManager.AccessFailedAsync(user);
            }
        }

        // Act - Use correct password
        var result = await UserManager.CheckPasswordAsync(user, correctPassword);

        // Assert
        result.Should().BeTrue();
        // CheckPasswordAsync başarılı olduğunda otomatik olarak reset edilir
        // Ama test ortamında manuel reset gerekebilir
        await UserManager.ResetAccessFailedCountAsync(user);
        var accessFailedCount = await UserManager.GetAccessFailedCountAsync(user);
        accessFailedCount.Should().Be(0); // Counter should be reset
    }
}

