using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using TE4IT.Domain.Constants;
using TE4IT.Infrastructure.Auth.Services;
using Xunit;

namespace TE4IT.Tests.Unit.Infrastructure.Auth.Services;

public class JwtTokenServiceTests
{
    private readonly IConfiguration _configuration;
    private readonly JwtTokenService _service;

    public JwtTokenServiceTests()
    {
        var configBuilder = new ConfigurationBuilder();
        configBuilder.AddInMemoryCollection(new Dictionary<string, string?>
        {
            { "Jwt:Issuer", "https://test-issuer.com" },
            { "Jwt:Audience", "https://test-audience.com" },
            { "Jwt:SigningKey", Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("test-signing-key-that-is-at-least-32-characters-long")) }
        });
        _configuration = configBuilder.Build();
        _service = new JwtTokenService(_configuration);
    }

    [Fact]
    public void CreateAccessToken_WithValidInput_ReturnsValidJwt()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var userName = "testuser";
        var email = "test@example.com";
        var roles = new[] { RoleNames.Administrator };
        var permissions = new[] { Permissions.Project.Create };

        // Act
        var (token, expiresAt) = _service.CreateAccessToken(userId, userName, email, roles, permissions);

        // Assert
        token.Should().NotBeNullOrEmpty();
        expiresAt.Should().BeAfter(DateTime.UtcNow);
        expiresAt.Should().BeCloseTo(DateTime.UtcNow.AddMinutes(15), TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void CreateAccessToken_ContainsCorrectClaims()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var userName = "testuser";
        var email = "test@example.com";
        var roles = new[] { RoleNames.Administrator, RoleNames.TeamLead };
        var permissions = new[] { Permissions.Project.Create, Permissions.Project.View };

        // Act
        var (token, _) = _service.CreateAccessToken(userId, userName, email, roles, permissions);

        // Assert
        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadJwtToken(token);

        jsonToken.Claims.Should().Contain(c => c.Type == ClaimTypes.NameIdentifier && c.Value == userId.ToString());
        jsonToken.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Sub && c.Value == userId.ToString());
        jsonToken.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Email && c.Value == email);
        jsonToken.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.UniqueName && c.Value == userName);
        jsonToken.Claims.Should().Contain(c => c.Type == ClaimTypes.Role && c.Value == RoleNames.Administrator);
        jsonToken.Claims.Should().Contain(c => c.Type == ClaimTypes.Role && c.Value == RoleNames.TeamLead);
        jsonToken.Claims.Should().Contain(c => c.Type == "permission" && c.Value == Permissions.Project.Create);
        jsonToken.Claims.Should().Contain(c => c.Type == "permission" && c.Value == Permissions.Project.View);
    }

    [Fact]
    public void CreateAccessToken_ExpiresIn15Minutes()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var beforeCreation = DateTime.UtcNow;

        // Act
        var (_, expiresAt) = _service.CreateAccessToken(userId, "test", "test@test.com", Array.Empty<string>(), Array.Empty<string>());

        // Assert
        var afterCreation = DateTime.UtcNow;
        expiresAt.Should().BeAfter(beforeCreation.AddMinutes(14));
        expiresAt.Should().BeBefore(afterCreation.AddMinutes(16));
    }

    [Fact]
    public void CreateAccessToken_WithPermissionsVersion_IncludesVersionClaim()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var permissionsVersion = "v1.0";

        // Act
        var (token, _) = _service.CreateAccessToken(userId, "test", "test@test.com", Array.Empty<string>(), Array.Empty<string>(), permissionsVersion);

        // Assert
        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadJwtToken(token);
        jsonToken.Claims.Should().Contain(c => c.Type == "permissions_version" && c.Value == permissionsVersion);
    }

    [Fact]
    public void CreateAccessToken_WithoutPermissionsVersion_ExcludesVersionClaim()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        var (token, _) = _service.CreateAccessToken(userId, "test", "test@test.com", Array.Empty<string>(), Array.Empty<string>(), null);

        // Assert
        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadJwtToken(token);
        jsonToken.Claims.Should().NotContain(c => c.Type == "permissions_version");
    }

    [Fact]
    public void CreateAccessToken_WithDifferentSigningKey_ProducesDifferentToken()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var config1 = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "Jwt:Issuer", "https://test-issuer.com" },
                { "Jwt:Audience", "https://test-audience.com" },
                { "Jwt:SigningKey", Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("key1-that-is-at-least-32-characters-long")) }
            })
            .Build();
        var config2 = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "Jwt:Issuer", "https://test-issuer.com" },
                { "Jwt:Audience", "https://test-audience.com" },
                { "Jwt:SigningKey", Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("key2-that-is-at-least-32-characters-long")) }
            })
            .Build();
        var service1 = new JwtTokenService(config1);
        var service2 = new JwtTokenService(config2);

        // Act
        var (token1, _) = service1.CreateAccessToken(userId, "test", "test@test.com", Array.Empty<string>(), Array.Empty<string>());
        var (token2, _) = service2.CreateAccessToken(userId, "test", "test@test.com", Array.Empty<string>(), Array.Empty<string>());

        // Assert
        token1.Should().NotBe(token2);
    }

    [Fact]
    public void CreateAccessToken_TokenIsSigned()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        var (token, _) = _service.CreateAccessToken(userId, "test", "test@test.com", Array.Empty<string>(), Array.Empty<string>());

        // Assert
        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadJwtToken(token);
        jsonToken.Header.Alg.Should().Be(SecurityAlgorithms.HmacSha256);
        jsonToken.Header.Typ.Should().Be("JWT");
        jsonToken.Claims.Should().NotBeEmpty();
    }

    [Fact]
    public void CreateAccessToken_ContainsJtiClaim()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        var (token, _) = _service.CreateAccessToken(userId, "test", "test@test.com", Array.Empty<string>(), Array.Empty<string>());

        // Assert
        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadJwtToken(token);
        jsonToken.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Jti);
        jsonToken.Id.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void CreateAccessToken_IssuerAndAudienceCorrect()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        var (token, _) = _service.CreateAccessToken(userId, "test", "test@test.com", Array.Empty<string>(), Array.Empty<string>());

        // Assert
        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadJwtToken(token);
        jsonToken.Issuer.Should().Be("https://test-issuer.com");
        jsonToken.Audiences.Should().Contain("https://test-audience.com");
    }

    [Fact]
    public void CreateRefreshToken_ReturnsUniqueTokens()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        var (token1, _) = _service.CreateRefreshToken(userId);
        var (token2, _) = _service.CreateRefreshToken(userId);

        // Assert
        token1.Should().NotBe(token2);
    }

    [Fact]
    public void CreateRefreshToken_ExpiresIn7Days()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var beforeCreation = DateTime.UtcNow;

        // Act
        var (_, expiresAt) = _service.CreateRefreshToken(userId);

        // Assert
        var afterCreation = DateTime.UtcNow;
        expiresAt.Should().BeAfter(beforeCreation.AddDays(6).AddHours(23));
        expiresAt.Should().BeBefore(afterCreation.AddDays(7).AddHours(1));
    }

    [Fact]
    public void CreateRefreshToken_IsBase64Encoded()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        var (token, _) = _service.CreateRefreshToken(userId);

        // Assert
        token.Should().NotBeNullOrEmpty();
        // Base64 string should only contain valid Base64 characters
        var base64Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=";
        token.All(c => base64Chars.Contains(c)).Should().BeTrue();
    }

    [Fact]
    public void CreateAccessToken_WithEmptyRoles_StillCreatesToken()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        var (token, _) = _service.CreateAccessToken(userId, "test", "test@test.com", Array.Empty<string>(), Array.Empty<string>());

        // Assert
        token.Should().NotBeNullOrEmpty();
        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadJwtToken(token);
        jsonToken.Claims.Where(c => c.Type == ClaimTypes.Role).Should().BeEmpty();
    }

    [Fact]
    public void CreateAccessToken_WithEmptyPermissions_StillCreatesToken()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        var (token, _) = _service.CreateAccessToken(userId, "test", "test@test.com", new[] { RoleNames.Administrator }, Array.Empty<string>());

        // Assert
        token.Should().NotBeNullOrEmpty();
        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadJwtToken(token);
        jsonToken.Claims.Where(c => c.Type == "permission").Should().BeEmpty();
    }

    [Fact]
    public void CreateAccessToken_WithNullUserName_HandlesGracefully()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        var (token, _) = _service.CreateAccessToken(userId, null!, "test@test.com", Array.Empty<string>(), Array.Empty<string>());

        // Assert
        token.Should().NotBeNullOrEmpty();
        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadJwtToken(token);
        jsonToken.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.UniqueName && c.Value == string.Empty);
    }
}

