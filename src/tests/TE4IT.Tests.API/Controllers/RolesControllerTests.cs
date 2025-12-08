using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using TE4IT.API;
using TE4IT.Application.Features.Auth.Commands.Roles.CreateRole;
using TE4IT.Application.Features.Auth.Queries.Roles.GetAllRoles;
using TE4IT.Tests.API.Common;
using Xunit;

namespace TE4IT.Tests.API.Controllers;

public class RolesControllerTests : ApiTestBase
{
    public RolesControllerTests(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    [Fact]
    public async Task GetAll_WithAuth_ReturnsOk()
    {
        // Arrange
        var token = await RegisterAndGetTokenAsync();
        SetAuthorizationHeader(token);

        // Act
        var response = await Client.GetAsync("/api/v1/roles");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<List<RoleResponse>>();
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GetAll_WithoutAuth_ReturnsUnauthorized()
    {
        // Arrange
        ClearAuthorizationHeader();

        // Act
        var response = await Client.GetAsync("/api/v1/roles");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetById_WithValidId_ReturnsOk()
    {
        // Arrange
        var token = await RegisterAndGetTokenAsync();
        SetAuthorizationHeader(token);

        // Önce bir rol oluştur
        var createRequest = new CreateRoleCommand("TestRole");
        var createResponse = await Client.PostAsJsonAsync("/api/v1/roles", createRequest);
        createResponse.EnsureSuccessStatusCode();
        var createdRole = await createResponse.Content.ReadFromJsonAsync<CreateRoleCommandResponse>();

        // Act
        var response = await Client.GetAsync($"/api/v1/roles/{createdRole!.RoleId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<RoleResponse>();
        result.Should().NotBeNull();
        result!.Id.Should().Be(createdRole.RoleId);
    }

    [Fact]
    public async Task GetById_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var token = await RegisterAndGetTokenAsync();
        SetAuthorizationHeader(token);
        var invalidId = Guid.NewGuid();

        // Act
        var response = await Client.GetAsync($"/api/v1/roles/{invalidId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Create_WithValidRequest_ReturnsCreated()
    {
        // Arrange
        var token = await RegisterAndGetTokenAsync();
        SetAuthorizationHeader(token);
        // Rol adı sadece harf içermeli (validator kuralı: ^[a-zA-Z]+$)
        // GUID veya sayı kullanamayız, sadece harf kullanmalıyız
        // Benzersizlik için timestamp'in son rakamlarını harfe çeviriyoruz
        var ticks = DateTime.UtcNow.Ticks;
        var uniqueSuffix = new string(Enumerable.Range(0, 10)
            .Select(i => (char)('A' + (int)((ticks >> (i * 4)) % 26)))
            .ToArray());
        var request = new CreateRoleCommand("TestRole" + uniqueSuffix);

        // Act
        var response = await Client.PostAsJsonAsync("/api/v1/roles", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var result = await response.Content.ReadFromJsonAsync<CreateRoleCommandResponse>();
        result.Should().NotBeNull();
        result!.RoleId.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Create_WithoutAuth_ReturnsUnauthorized()
    {
        // Arrange
        ClearAuthorizationHeader();
        var request = new CreateRoleCommand("TestRole");

        // Act
        var response = await Client.PostAsJsonAsync("/api/v1/roles", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}

