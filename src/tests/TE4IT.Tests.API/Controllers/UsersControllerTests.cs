using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using TE4IT.API;
using TE4IT.Application.Common.Pagination;
using TE4IT.Application.Features.Auth.Queries.Users.GetAllUsers;
using TE4IT.Application.Features.Auth.Queries.Users.GetUserById;
using TE4IT.Tests.API.Common;
using Xunit;

namespace TE4IT.Tests.API.Controllers;

public class UsersControllerTests : ApiTestBase
{
    public UsersControllerTests(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    [Fact]
    public async Task GetAll_WithAuth_ReturnsOk()
    {
        // Arrange
        var token = await RegisterAndGetTokenAsync();
        SetAuthorizationHeader(token);

        // Act
        var response = await Client.GetAsync("/api/v1/users");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PagedResult<UserResponse>>();
        result.Should().NotBeNull();
        result!.Items.Should().NotBeNull();
    }

    [Fact]
    public async Task GetAll_WithoutAuth_ReturnsUnauthorized()
    {
        // Arrange
        ClearAuthorizationHeader();

        // Act
        var response = await Client.GetAsync("/api/v1/users");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetById_WithValidId_ReturnsOk()
    {
        // Arrange
        var token = await RegisterAndGetTokenAsync();
        SetAuthorizationHeader(token);

        // Kullanıcı ID'sini register response'dan al
        // UserName sadece harf, rakam ve alt çizgi içerebilir (validation kuralı: ^[a-zA-Z0-9_]+$)
        // GUID kullanamayız çünkü tire (-) içeriyor, bunun yerine timestamp kullanıyoruz
        var uniqueId = DateTime.UtcNow.Ticks;
        var registerResponse = await Client.PostAsJsonAsync("/api/v1/auth/register",
            new TE4IT.Application.Features.Auth.Commands.Register.RegisterCommand(
                $"user{uniqueId}",
                $"user{uniqueId}@example.com",
                "Test123!@#"));
        registerResponse.EnsureSuccessStatusCode();
        var registerResult = await registerResponse.Content.ReadFromJsonAsync<TE4IT.Application.Features.Auth.Commands.Register.RegisterCommandResponse>();

        // Act
        var response = await Client.GetAsync($"/api/v1/users/{registerResult!.UserId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<UserResponse>();
        result.Should().NotBeNull();
        result!.Id.Should().Be(registerResult.UserId);
    }

    [Fact]
    public async Task GetById_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var token = await RegisterAndGetTokenAsync();
        SetAuthorizationHeader(token);
        var invalidId = Guid.NewGuid();

        // Act
        var response = await Client.GetAsync($"/api/v1/users/{invalidId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetUserRoles_WithValidId_ReturnsOk()
    {
        // Arrange
        var token = await RegisterAndGetTokenAsync();
        SetAuthorizationHeader(token);

        // Kullanıcı ID'sini register response'dan al
        // UserName sadece harf, rakam ve alt çizgi içerebilir (validation kuralı)
        var uniqueId = DateTime.UtcNow.Ticks.ToString().Replace("0", "a").Replace("1", "b").Replace("2", "c").Replace("3", "d").Replace("4", "e").Replace("5", "f").Replace("6", "g").Replace("7", "h").Replace("8", "i").Replace("9", "j");
        var registerResponse = await Client.PostAsJsonAsync("/api/v1/auth/register",
            new TE4IT.Application.Features.Auth.Commands.Register.RegisterCommand(
                $"user{uniqueId}",
                $"user{uniqueId}@example.com",
                "Test123!@#"));
        registerResponse.EnsureSuccessStatusCode();
        var registerResult = await registerResponse.Content.ReadFromJsonAsync<TE4IT.Application.Features.Auth.Commands.Register.RegisterCommandResponse>();

        // Act
        var response = await Client.GetAsync($"/api/v1/users/{registerResult!.UserId}/roles");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<List<string>>();
        result.Should().NotBeNull();
    }
}

