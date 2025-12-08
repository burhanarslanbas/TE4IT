using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using TE4IT.API;
using TE4IT.Application.Features.Auth.Commands.Login;
using TE4IT.Application.Features.Auth.Commands.Register;
using TE4IT.Tests.API.Common;
using Xunit;

namespace TE4IT.Tests.API.Controllers;

public class AuthControllerTests : ApiTestBase
{
    public AuthControllerTests(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    [Fact]
    public async Task Register_WithValidRequest_ReturnsCreated()
    {
        // Arrange
        ClearAuthorizationHeader();
        var uniqueEmail = $"test{Guid.NewGuid()}@example.com";
        var request = new RegisterCommand("testuser", uniqueEmail, "Test123!@#");

        // Act
        var response = await Client.PostAsJsonAsync("/api/v1/auth/register", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var result = await response.Content.ReadFromJsonAsync<RegisterCommandResponse>();
        result.Should().NotBeNull();
        result!.UserId.Should().NotBeEmpty();
        result.Email.Should().Be(uniqueEmail);
        result.AccessToken.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Register_WithDuplicateEmail_ReturnsBadRequest()
    {
        // Arrange
        ClearAuthorizationHeader();
        var email = $"duplicate{Guid.NewGuid()}@example.com";
        var request1 = new RegisterCommand("user1", email, "Test123!@#");
        var request2 = new RegisterCommand("user2", email, "Test123!@#");

        // İlk kayıt
        await Client.PostAsJsonAsync("/api/v1/auth/register", request1);

        // Act - İkinci kayıt (aynı email)
        var response = await Client.PostAsJsonAsync("/api/v1/auth/register", request2);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsOk()
    {
        // Arrange
        ClearAuthorizationHeader();
        var email = $"login{Guid.NewGuid()}@example.com";
        var password = "Test123!@#";
        
        // Önce kullanıcı kaydı yap
        var registerRequest = new RegisterCommand("loginuser", email, password);
        await Client.PostAsJsonAsync("/api/v1/auth/register", registerRequest);

        // Act
        var loginRequest = new LoginCommand(email, password);
        var response = await Client.PostAsJsonAsync("/api/v1/auth/login", loginRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<LoginCommandResponse>();
        result.Should().NotBeNull();
        result!.AccessToken.Should().NotBeNullOrEmpty();
        result.RefreshToken.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Login_WithInvalidCredentials_ReturnsUnauthorized()
    {
        // Arrange
        ClearAuthorizationHeader();
        var loginRequest = new LoginCommand("nonexistent@example.com", "WrongPassword123!@#");

        // Act
        var response = await Client.PostAsJsonAsync("/api/v1/auth/login", loginRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_WithWrongPassword_ReturnsUnauthorized()
    {
        // Arrange
        ClearAuthorizationHeader();
        var email = $"wrongpass{Guid.NewGuid()}@example.com";
        var password = "Test123!@#";
        
        // Kullanıcı kaydı yap
        var registerRequest = new RegisterCommand("wrongpassuser", email, password);
        await Client.PostAsJsonAsync("/api/v1/auth/register", registerRequest);

        // Act - Yanlış şifre ile login
        var loginRequest = new LoginCommand(email, "WrongPassword123!@#");
        var response = await Client.PostAsJsonAsync("/api/v1/auth/login", loginRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}

