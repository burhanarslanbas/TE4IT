using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using TE4IT.API;
using TE4IT.Application.Common.Pagination;
using TE4IT.Application.Features.Modules.Commands.CreateModule;
using TE4IT.Application.Features.Modules.Responses;
using TE4IT.Application.Features.Projects.Commands.CreateProject;
using TE4IT.Tests.API.Common;
using Xunit;

namespace TE4IT.Tests.API.Controllers;

public class ModulesControllerTests : ApiTestBase
{
    public ModulesControllerTests(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    [Fact]
    public async Task Create_WithValidRequest_ReturnsCreated()
    {
        // Arrange
        var token = await RegisterAndGetTokenAsync();
        SetAuthorizationHeader(token);

        // Önce bir proje oluştur
        var createProjectRequest = new CreateProjectCommand("Test Project", "Test Description");
        var projectResponse = await Client.PostAsJsonAsync("/api/v1/projects", createProjectRequest);
        projectResponse.EnsureSuccessStatusCode();
        var project = await projectResponse.Content.ReadFromJsonAsync<CreateProjectCommandResponse>();

        var request = new { Title = "Test Module", Description = "Test Description" };

        // Act
        var response = await Client.PostAsJsonAsync($"/api/v1/modules/projects/{project!.Id}", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var result = await response.Content.ReadFromJsonAsync<CreateModuleCommandResponse>();
        result.Should().NotBeNull();
        result!.Id.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Create_WithoutAuth_ReturnsUnauthorized()
    {
        // Arrange
        ClearAuthorizationHeader();
        var request = new { Title = "Test Module", Description = "Test Description" };

        // Act
        var response = await Client.PostAsJsonAsync($"/api/v1/modules/projects/{Guid.NewGuid()}", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetById_WithValidId_ReturnsOk()
    {
        // Arrange
        var token = await RegisterAndGetTokenAsync();
        SetAuthorizationHeader(token);

        // Proje ve modül oluştur
        var createProjectRequest = new CreateProjectCommand("Test Project", "Test Description");
        var projectResponse = await Client.PostAsJsonAsync("/api/v1/projects", createProjectRequest);
        projectResponse.EnsureSuccessStatusCode();
        var project = await projectResponse.Content.ReadFromJsonAsync<CreateProjectCommandResponse>();

        var createModuleRequest = new { Title = "Test Module", Description = "Test Description" };
        var moduleResponse = await Client.PostAsJsonAsync($"/api/v1/modules/projects/{project!.Id}", createModuleRequest);
        moduleResponse.EnsureSuccessStatusCode();
        var module = await moduleResponse.Content.ReadFromJsonAsync<CreateModuleCommandResponse>();

        // Act
        var response = await Client.GetAsync($"/api/v1/modules/{module!.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<ModuleResponse>();
        result.Should().NotBeNull();
        result!.Id.Should().Be(module.Id);
    }

    [Fact]
    public async Task GetByProject_ReturnsPagedResults()
    {
        // Arrange
        var token = await RegisterAndGetTokenAsync();
        SetAuthorizationHeader(token);

        // Proje oluştur
        var createProjectRequest = new CreateProjectCommand("Test Project", "Test Description");
        var projectResponse = await Client.PostAsJsonAsync("/api/v1/projects", createProjectRequest);
        projectResponse.EnsureSuccessStatusCode();
        var project = await projectResponse.Content.ReadFromJsonAsync<CreateProjectCommandResponse>();

        // Birkaç modül oluştur
        for (int i = 0; i < 3; i++)
        {
            var createModuleRequest = new { Title = $"Module {i}", Description = $"Description {i}" };
            await Client.PostAsJsonAsync($"/api/v1/modules/projects/{project!.Id}", createModuleRequest);
        }

        // Act
        var response = await Client.GetAsync($"/api/v1/modules/projects/{project!.Id}?page=1&pageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PagedResult<ModuleListItemResponse>>();
        result.Should().NotBeNull();
        result!.Items.Should().HaveCountGreaterThanOrEqualTo(3);
    }

    [Fact]
    public async Task UpdateModule_WithValidRequest_ReturnsNoContent()
    {
        // Arrange
        var token = await RegisterAndGetTokenAsync();
        SetAuthorizationHeader(token);

        // Proje ve modül oluştur
        var createProjectRequest = new CreateProjectCommand("Test Project", "Test Description");
        var projectResponse = await Client.PostAsJsonAsync("/api/v1/projects", createProjectRequest);
        projectResponse.EnsureSuccessStatusCode();
        var project = await projectResponse.Content.ReadFromJsonAsync<CreateProjectCommandResponse>();

        var createModuleRequest = new { Title = "Test Module", Description = "Test Description" };
        var moduleResponse = await Client.PostAsJsonAsync($"/api/v1/modules/projects/{project!.Id}", createModuleRequest);
        moduleResponse.EnsureSuccessStatusCode();
        var module = await moduleResponse.Content.ReadFromJsonAsync<CreateModuleCommandResponse>();

        var updateRequest = new { Title = "Updated Module", Description = "Updated Description" };

        // Act
        var response = await Client.PutAsJsonAsync($"/api/v1/modules/{module!.Id}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task UpdateModule_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var token = await RegisterAndGetTokenAsync();
        SetAuthorizationHeader(token);
        var invalidId = Guid.NewGuid();

        var updateRequest = new { Title = "Updated Module", Description = "Updated Description" };

        // Act
        var response = await Client.PutAsJsonAsync($"/api/v1/modules/{invalidId}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ChangeModuleStatus_WithValidRequest_ReturnsNoContent()
    {
        // Arrange
        var token = await RegisterAndGetTokenAsync();
        SetAuthorizationHeader(token);

        // Proje ve modül oluştur
        var createProjectRequest = new CreateProjectCommand("Test Project", "Test Description");
        var projectResponse = await Client.PostAsJsonAsync("/api/v1/projects", createProjectRequest);
        projectResponse.EnsureSuccessStatusCode();
        var project = await projectResponse.Content.ReadFromJsonAsync<CreateProjectCommandResponse>();

        var createModuleRequest = new { Title = "Test Module", Description = "Test Description" };
        var moduleResponse = await Client.PostAsJsonAsync($"/api/v1/modules/projects/{project!.Id}", createModuleRequest);
        moduleResponse.EnsureSuccessStatusCode();
        var module = await moduleResponse.Content.ReadFromJsonAsync<CreateModuleCommandResponse>();

        var changeStatusRequest = new { IsActive = false };

        // Act
        var response = await Client.PatchAsJsonAsync($"/api/v1/modules/{module!.Id}/status", changeStatusRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task ChangeModuleStatus_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var token = await RegisterAndGetTokenAsync();
        SetAuthorizationHeader(token);
        var invalidId = Guid.NewGuid();

        var changeStatusRequest = new { IsActive = false };

        // Act
        var response = await Client.PatchAsJsonAsync($"/api/v1/modules/{invalidId}/status", changeStatusRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteModule_WithValidId_ReturnsNoContent()
    {
        // Arrange
        var token = await RegisterAndGetTokenAsync();
        SetAuthorizationHeader(token);

        // Proje ve modül oluştur
        var createProjectRequest = new CreateProjectCommand("Test Project", "Test Description");
        var projectResponse = await Client.PostAsJsonAsync("/api/v1/projects", createProjectRequest);
        projectResponse.EnsureSuccessStatusCode();
        var project = await projectResponse.Content.ReadFromJsonAsync<CreateProjectCommandResponse>();

        var createModuleRequest = new { Title = "Test Module", Description = "Test Description" };
        var moduleResponse = await Client.PostAsJsonAsync($"/api/v1/modules/projects/{project!.Id}", createModuleRequest);
        moduleResponse.EnsureSuccessStatusCode();
        var module = await moduleResponse.Content.ReadFromJsonAsync<CreateModuleCommandResponse>();

        // Act
        var response = await Client.DeleteAsync($"/api/v1/modules/{module!.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteModule_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var token = await RegisterAndGetTokenAsync();
        SetAuthorizationHeader(token);
        var invalidId = Guid.NewGuid();

        // Act
        var response = await Client.DeleteAsync($"/api/v1/modules/{invalidId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}

