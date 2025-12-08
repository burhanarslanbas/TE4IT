using System.Security.Cryptography;
using System.Text;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TE4IT.Application.Abstractions.Auth;
using TE4IT.Persistence.Common.Identity;
using TE4IT.Tests.Integration.Common;
using Xunit;

namespace TE4IT.Tests.Integration.Infrastructure.Auth.Services;

public class RefreshTokenServiceTests : IntegrationTestBase
{
    private readonly IRefreshTokenService _service;

    public RefreshTokenServiceTests()
    {
        _service = new TE4IT.Infrastructure.Auth.Services.RefreshTokenService(DbContext);
    }

    [Fact]
    public async Task IssueAsync_WithValidUserId_CreatesRefreshToken()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var ipAddress = "192.168.1.1";

        // Act
        var (token, expiresAt) = await _service.IssueAsync(userId, ipAddress, CancellationToken.None);

        // Assert
        token.Should().NotBeNullOrEmpty();
        expiresAt.Should().BeAfter(DateTime.UtcNow);
        
        var savedToken = await DbContext.Set<RefreshToken>().FirstOrDefaultAsync(rt => rt.UserId == userId);
        savedToken.Should().NotBeNull();
        savedToken!.Token.Should().Be(token);
    }

    [Fact]
    public async Task IssueAsync_TokenIsHashed()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var ipAddress = "192.168.1.1";

        // Act
        var (token, _) = await _service.IssueAsync(userId, ipAddress, CancellationToken.None);

        // Assert
        var savedToken = await DbContext.Set<RefreshToken>().FirstOrDefaultAsync(rt => rt.Token == token);
        savedToken.Should().NotBeNull();
        savedToken!.TokenHash.Should().NotBeNullOrEmpty();
        
        // Verify hash is SHA256
        var expectedHash = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
        savedToken.TokenHash.Should().Be(expectedHash);
    }

    [Fact]
    public async Task IssueAsync_StoresInDatabase()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var ipAddress = "192.168.1.1";

        // Act
        var (token, _) = await _service.IssueAsync(userId, ipAddress, CancellationToken.None);

        // Assert
        var count = await DbContext.Set<RefreshToken>().CountAsync();
        count.Should().Be(1);
        
        var savedToken = await DbContext.Set<RefreshToken>().FirstOrDefaultAsync();
        savedToken.Should().NotBeNull();
        savedToken!.UserId.Should().Be(userId);
        savedToken.Token.Should().Be(token);
    }

    [Fact]
    public async Task IssueAsync_ExpiresIn7Days()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var ipAddress = "192.168.1.1";
        var beforeCreation = DateTime.UtcNow;

        // Act
        var (_, expiresAt) = await _service.IssueAsync(userId, ipAddress, CancellationToken.None);

        // Assert
        var afterCreation = DateTime.UtcNow;
        expiresAt.Should().BeAfter(beforeCreation.AddDays(6).AddHours(23));
        expiresAt.Should().BeBefore(afterCreation.AddDays(7).AddHours(1));
    }

    [Fact]
    public async Task IssueAsync_RecordsIpAddress()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var ipAddress = "192.168.1.1";

        // Act
        var (token, _) = await _service.IssueAsync(userId, ipAddress, CancellationToken.None);

        // Assert
        var savedToken = await DbContext.Set<RefreshToken>().FirstOrDefaultAsync(rt => rt.Token == token);
        savedToken.Should().NotBeNull();
        savedToken!.CreatedByIp.Should().Be(ipAddress);
    }

    [Fact]
    public async Task RefreshAsync_WithValidToken_ReturnsNewTokens()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var ipAddress = "192.168.1.1";
        var (oldToken, _) = await _service.IssueAsync(userId, ipAddress, CancellationToken.None);

        // Act
        var result = await _service.RefreshAsync(oldToken, ipAddress, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Value.userId.Should().Be(userId);
        result.Value.refreshToken.Should().NotBeNullOrEmpty();
        result.Value.refreshToken.Should().NotBe(oldToken);
    }

    [Fact]
    public async Task RefreshAsync_RevokesOldToken()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var ipAddress = "192.168.1.1";
        var (oldToken, _) = await _service.IssueAsync(userId, ipAddress, CancellationToken.None);

        // Act
        await _service.RefreshAsync(oldToken, ipAddress, CancellationToken.None);

        // Assert
        var oldTokenHash = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(oldToken)));
        var revokedToken = await DbContext.Set<RefreshToken>().FirstOrDefaultAsync(rt => rt.TokenHash == oldTokenHash);
        revokedToken.Should().NotBeNull();
        revokedToken!.RevokedAt.Should().NotBeNull();
        revokedToken.RevokedByIp.Should().Be(ipAddress);
    }

    [Fact]
    public async Task RefreshAsync_CreatesNewToken()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var ipAddress = "192.168.1.1";
        var (oldToken, _) = await _service.IssueAsync(userId, ipAddress, CancellationToken.None);

        // Act
        var result = await _service.RefreshAsync(oldToken, ipAddress, CancellationToken.None);

        // Assert
        var newToken = result.Value.refreshToken;
        var newTokenHash = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(newToken)));
        var savedNewToken = await DbContext.Set<RefreshToken>().FirstOrDefaultAsync(rt => rt.TokenHash == newTokenHash);
        savedNewToken.Should().NotBeNull();
        savedNewToken!.RevokedAt.Should().BeNull();
    }

    [Fact]
    public async Task RefreshAsync_NewTokenIsDifferent()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var ipAddress = "192.168.1.1";
        var (oldToken, _) = await _service.IssueAsync(userId, ipAddress, CancellationToken.None);

        // Act
        var result = await _service.RefreshAsync(oldToken, ipAddress, CancellationToken.None);

        // Assert
        result.Value.refreshToken.Should().NotBe(oldToken);
    }

    [Fact]
    public async Task RefreshAsync_RecordsReplacement()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var ipAddress = "192.168.1.1";
        var (oldToken, _) = await _service.IssueAsync(userId, ipAddress, CancellationToken.None);

        // Act
        var result = await _service.RefreshAsync(oldToken, ipAddress, CancellationToken.None);

        // Assert
        var oldTokenHash = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(oldToken)));
        var revokedToken = await DbContext.Set<RefreshToken>().FirstOrDefaultAsync(rt => rt.TokenHash == oldTokenHash);
        revokedToken.Should().NotBeNull();
        revokedToken!.ReplacedByToken.Should().Be(result.Value.refreshToken);
    }

    [Fact]
    public async Task RefreshAsync_WithExpiredToken_ReturnsNull()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var ipAddress = "192.168.1.1";
        var (token, _) = await _service.IssueAsync(userId, ipAddress, CancellationToken.None);
        
        // Manually expire the token
        var tokenHash = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
        var refreshToken = await DbContext.Set<RefreshToken>().FirstOrDefaultAsync(rt => rt.TokenHash == tokenHash);
        refreshToken!.ExpiresAt = DateTime.UtcNow.AddDays(-1);
        await DbContext.SaveChangesAsync();

        // Act
        var result = await _service.RefreshAsync(token, ipAddress, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task RefreshAsync_WithRevokedToken_ReturnsNull()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var ipAddress = "192.168.1.1";
        var (token, _) = await _service.IssueAsync(userId, ipAddress, CancellationToken.None);
        await _service.RevokeAsync(token, ipAddress, "Test revocation", CancellationToken.None);

        // Act
        var result = await _service.RefreshAsync(token, ipAddress, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task RefreshAsync_WithInvalidToken_ReturnsNull()
    {
        // Arrange
        var invalidToken = "invalid-token";
        var ipAddress = "192.168.1.1";

        // Act
        var result = await _service.RefreshAsync(invalidToken, ipAddress, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task RefreshAsync_WithSameTokenTwice_SecondCallFails()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var ipAddress = "192.168.1.1";
        var (token, _) = await _service.IssueAsync(userId, ipAddress, CancellationToken.None);

        // Act
        var firstResult = await _service.RefreshAsync(token, ipAddress, CancellationToken.None);
        var secondResult = await _service.RefreshAsync(token, ipAddress, CancellationToken.None);

        // Assert
        firstResult.Should().NotBeNull();
        secondResult.Should().BeNull(); // Token already revoked in first call
    }

    [Fact]
    public async Task RevokeAsync_WithValidToken_ReturnsTrue()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var ipAddress = "192.168.1.1";
        var (token, _) = await _service.IssueAsync(userId, ipAddress, CancellationToken.None);

        // Act
        var result = await _service.RevokeAsync(token, ipAddress, "Test reason", CancellationToken.None);

        // Assert
        result.Should().BeTrue();
        
        var tokenHash = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
        var revokedToken = await DbContext.Set<RefreshToken>().FirstOrDefaultAsync(rt => rt.TokenHash == tokenHash);
        revokedToken.Should().NotBeNull();
        revokedToken!.RevokedAt.Should().NotBeNull();
        revokedToken.RevokeReason.Should().Be("Test reason");
    }

    [Fact]
    public async Task RevokeAsync_RevokedTokenCannotBeUsed()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var ipAddress = "192.168.1.1";
        var (token, _) = await _service.IssueAsync(userId, ipAddress, CancellationToken.None);
        await _service.RevokeAsync(token, ipAddress, null, CancellationToken.None);

        // Act
        var refreshResult = await _service.RefreshAsync(token, ipAddress, CancellationToken.None);

        // Assert
        refreshResult.Should().BeNull();
    }

    [Fact]
    public async Task RevokeAsync_WithInvalidToken_ReturnsFalse()
    {
        // Arrange
        var invalidToken = "invalid-token";
        var ipAddress = "192.168.1.1";

        // Act
        var result = await _service.RevokeAsync(invalidToken, ipAddress, null, CancellationToken.None);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task RevokeAsync_WithAlreadyRevokedToken_ReturnsFalse()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var ipAddress = "192.168.1.1";
        var (token, _) = await _service.IssueAsync(userId, ipAddress, CancellationToken.None);
        await _service.RevokeAsync(token, ipAddress, null, CancellationToken.None);

        // Act
        var result = await _service.RevokeAsync(token, ipAddress, null, CancellationToken.None);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task RevokeAsync_RecordsRevokeReason()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var ipAddress = "192.168.1.1";
        var (token, _) = await _service.IssueAsync(userId, ipAddress, CancellationToken.None);
        var reason = "User logged out";

        // Act
        await _service.RevokeAsync(token, ipAddress, reason, CancellationToken.None);

        // Assert
        var tokenHash = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
        var revokedToken = await DbContext.Set<RefreshToken>().FirstOrDefaultAsync(rt => rt.TokenHash == tokenHash);
        revokedToken.Should().NotBeNull();
        revokedToken!.RevokeReason.Should().Be(reason);
    }

    [Fact]
    public async Task RefreshAsync_PreventsTokenReuse()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var ipAddress = "192.168.1.1";
        var (token, _) = await _service.IssueAsync(userId, ipAddress, CancellationToken.None);

        // Act - First refresh
        var firstResult = await _service.RefreshAsync(token, ipAddress, CancellationToken.None);
        
        // Act - Try to use old token again
        var secondResult = await _service.RefreshAsync(token, ipAddress, CancellationToken.None);

        // Assert
        firstResult.Should().NotBeNull();
        secondResult.Should().BeNull(); // Old token cannot be reused
    }

    [Fact]
    public async Task RefreshAsync_TokenHashIsSecure()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var ipAddress = "192.168.1.1";
        var (token, _) = await _service.IssueAsync(userId, ipAddress, CancellationToken.None);

        // Act
        var tokenHash = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(token)));

        // Assert
        var savedToken = await DbContext.Set<RefreshToken>().FirstOrDefaultAsync(rt => rt.Token == token);
        savedToken.Should().NotBeNull();
        savedToken!.TokenHash.Should().Be(tokenHash);
        savedToken.TokenHash.Should().NotBe(token); // Hash is different from original token
    }

    [Fact]
    public async Task RefreshAsync_WithStolenToken_DetectsReuse()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var ipAddress1 = "192.168.1.1";
        var ipAddress2 = "192.168.1.2";
        var (token, _) = await _service.IssueAsync(userId, ipAddress1, CancellationToken.None);

        // Act - First use (legitimate)
        var firstResult = await _service.RefreshAsync(token, ipAddress1, CancellationToken.None);
        
        // Act - Second use (stolen token attempt)
        var secondResult = await _service.RefreshAsync(token, ipAddress2, CancellationToken.None);

        // Assert
        firstResult.Should().NotBeNull();
        secondResult.Should().BeNull(); // Stolen token cannot be used
    }

    [Fact]
    public async Task RefreshAsync_ConcurrentRefresh_HandlesCorrectly()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var ipAddress = "192.168.1.1";
        var (token, _) = await _service.IssueAsync(userId, ipAddress, CancellationToken.None);

        // Act - Simulate concurrent refresh attempts
        var tasks = new List<Task<(Guid userId, string accessToken, DateTime accessExpiresAt, string refreshToken, DateTime refreshExpiresAt)?>>();
        for (int i = 0; i < 5; i++)
        {
            tasks.Add(_service.RefreshAsync(token, ipAddress, CancellationToken.None));
        }
        var results = await Task.WhenAll(tasks);

        // Assert - Only one should succeed
        var successfulResults = results.Where(r => r != null).ToList();
        successfulResults.Should().HaveCount(1);
        
        // All tokens should be revoked
        var tokenHash = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
        var revokedTokens = await DbContext.Set<RefreshToken>()
            .Where(rt => rt.TokenHash == tokenHash || rt.ReplacedByToken == successfulResults[0]!.Value.refreshToken)
            .ToListAsync();
        revokedTokens.Should().HaveCount(1); // Original token revoked
    }
}

