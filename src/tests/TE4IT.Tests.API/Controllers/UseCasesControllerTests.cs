using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using TE4IT.API;
using TE4IT.Application.Common.Pagination;
using TE4IT.Application.Features.Modules.Commands.CreateModule;
using TE4IT.Application.Features.Projects.Commands.CreateProject;
using TE4IT.Application.Features.UseCases.Commands.CreateUseCase;
using TE4IT.Application.Features.UseCases.Responses;
using TE4IT.Tests.API.Common;
using Xunit;

namespace TE4IT.Tests.API.Controllers;

public class UseCasesControllerTests : ApiTestBase
{
    public UseCasesControllerTests(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    [Fact]
    public async Task Create_WithValidRequest_ReturnsCreated()
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

        var request = new { Title = "Test UseCase", Description = "Test Description", ImportantNotes = "Important" };

        // Act
        var response = await Client.PostAsJsonAsync($"/api/v1/usecases/modules/{module!.Id}", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var result = await response.Content.ReadFromJsonAsync<CreateUseCaseCommandResponse>();
        result.Should().NotBeNull();
        result!.Id.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetById_WithValidId_ReturnsOk()
    {
        // Arrange
        var token = await RegisterAndGetTokenAsync();
        SetAuthorizationHeader(token);

        // Proje, modül ve use case oluştur
        var createProjectRequest = new CreateProjectCommand("Test Project", "Test Description");
        var projectResponse = await Client.PostAsJsonAsync("/api/v1/projects", createProjectRequest);
        projectResponse.EnsureSuccessStatusCode();
        var project = await projectResponse.Content.ReadFromJsonAsync<CreateProjectCommandResponse>();

        var createModuleRequest = new { Title = "Test Module", Description = "Test Description" };
        var moduleResponse = await Client.PostAsJsonAsync($"/api/v1/modules/projects/{project!.Id}", createModuleRequest);
        moduleResponse.EnsureSuccessStatusCode();
        var module = await moduleResponse.Content.ReadFromJsonAsync<CreateModuleCommandResponse>();

        var createUseCaseRequest = new { Title = "Test UseCase", Description = "Test Description", ImportantNotes = "Important" };
        var useCaseResponse = await Client.PostAsJsonAsync($"/api/v1/usecases/modules/{module!.Id}", createUseCaseRequest);
        useCaseResponse.EnsureSuccessStatusCode();
        var useCase = await useCaseResponse.Content.ReadFromJsonAsync<CreateUseCaseCommandResponse>();

        // Act
        var response = await Client.GetAsync($"/api/v1/usecases/{useCase!.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<UseCaseResponse>();
        result.Should().NotBeNull();
        result!.Id.Should().Be(useCase.Id);
    }

    [Fact]
    public async Task GetByModule_ReturnsPagedResults()
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

        // Birkaç use case oluştur
        for (int i = 0; i < 3; i++)
        {
            var createUseCaseRequest = new { Title = $"UseCase {i}", Description = $"Description {i}", ImportantNotes = (string?)null };
            await Client.PostAsJsonAsync($"/api/v1/usecases/modules/{module!.Id}", createUseCaseRequest);
        }

        // Act
        var response = await Client.GetAsync($"/api/v1/usecases/modules/{module!.Id}?page=1&pageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PagedResult<UseCaseListItemResponse>>();
        result.Should().NotBeNull();
        result!.Items.Should().HaveCountGreaterThanOrEqualTo(3);
    }

    [Fact]
    public async Task UpdateUseCase_WithValidRequest_ReturnsNoContent()
    {
        // Arrange
        var token = await RegisterAndGetTokenAsync();
        SetAuthorizationHeader(token);

        // Proje, modül ve use case oluştur
        var createProjectRequest = new CreateProjectCommand("Test Project", "Test Description");
        var projectResponse = await Client.PostAsJsonAsync("/api/v1/projects", createProjectRequest);
        projectResponse.EnsureSuccessStatusCode();
        var project = await projectResponse.Content.ReadFromJsonAsync<CreateProjectCommandResponse>();

        var createModuleRequest = new { Title = "Test Module", Description = "Test Description" };
        var moduleResponse = await Client.PostAsJsonAsync($"/api/v1/modules/projects/{project!.Id}", createModuleRequest);
        moduleResponse.EnsureSuccessStatusCode();
        var module = await moduleResponse.Content.ReadFromJsonAsync<CreateModuleCommandResponse>();

        var createUseCaseRequest = new { Title = "Test UseCase", Description = "Test Description", ImportantNotes = "Important" };
        var useCaseResponse = await Client.PostAsJsonAsync($"/api/v1/usecases/modules/{module!.Id}", createUseCaseRequest);
        useCaseResponse.EnsureSuccessStatusCode();
        var useCase = await useCaseResponse.Content.ReadFromJsonAsync<CreateUseCaseCommandResponse>();

        var updateRequest = new { Title = "Updated UseCase", Description = "Updated Description", ImportantNotes = "Updated Notes" };

        // Act
        var response = await Client.PutAsJsonAsync($"/api/v1/usecases/{useCase!.Id}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task UpdateUseCase_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var token = await RegisterAndGetTokenAsync();
        SetAuthorizationHeader(token);
        var invalidId = Guid.NewGuid();

        var updateRequest = new { Title = "Updated UseCase", Description = "Updated Description", ImportantNotes = (string?)null };

        // Act
        var response = await Client.PutAsJsonAsync($"/api/v1/usecases/{invalidId}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ChangeUseCaseStatus_WithValidRequest_ReturnsNoContent()
    {
        // Arrange
        var token = await RegisterAndGetTokenAsync();
        SetAuthorizationHeader(token);

        // Proje, modül ve use case oluştur
        var createProjectRequest = new CreateProjectCommand("Test Project", "Test Description");
        var projectResponse = await Client.PostAsJsonAsync("/api/v1/projects", createProjectRequest);
        projectResponse.EnsureSuccessStatusCode();
        var project = await projectResponse.Content.ReadFromJsonAsync<CreateProjectCommandResponse>();

        var createModuleRequest = new { Title = "Test Module", Description = "Test Description" };
        var moduleResponse = await Client.PostAsJsonAsync($"/api/v1/modules/projects/{project!.Id}", createModuleRequest);
        moduleResponse.EnsureSuccessStatusCode();
        var module = await moduleResponse.Content.ReadFromJsonAsync<CreateModuleCommandResponse>();

        var createUseCaseRequest = new { Title = "Test UseCase", Description = "Test Description", ImportantNotes = (string?)null };
        var useCaseResponse = await Client.PostAsJsonAsync($"/api/v1/usecases/modules/{module!.Id}", createUseCaseRequest);
        useCaseResponse.EnsureSuccessStatusCode();
        var useCase = await useCaseResponse.Content.ReadFromJsonAsync<CreateUseCaseCommandResponse>();

        var changeStatusRequest = new { IsActive = false };

        // Act
        var response = await Client.PatchAsJsonAsync($"/api/v1/usecases/{useCase!.Id}/status", changeStatusRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task ChangeUseCaseStatus_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var token = await RegisterAndGetTokenAsync();
        SetAuthorizationHeader(token);
        var invalidId = Guid.NewGuid();

        var changeStatusRequest = new { IsActive = false };

        // Act
        var response = await Client.PatchAsJsonAsync($"/api/v1/usecases/{invalidId}/status", changeStatusRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteUseCase_WithValidId_ReturnsNoContent()
    {
        // Arrange
        var token = await RegisterAndGetTokenAsync();
        SetAuthorizationHeader(token);

        // Proje, modül ve use case oluştur
        var createProjectRequest = new CreateProjectCommand("Test Project", "Test Description");
        var projectResponse = await Client.PostAsJsonAsync("/api/v1/projects", createProjectRequest);
        projectResponse.EnsureSuccessStatusCode();
        var project = await projectResponse.Content.ReadFromJsonAsync<CreateProjectCommandResponse>();

        var createModuleRequest = new { Title = "Test Module", Description = "Test Description" };
        var moduleResponse = await Client.PostAsJsonAsync($"/api/v1/modules/projects/{project!.Id}", createModuleRequest);
        moduleResponse.EnsureSuccessStatusCode();
        var module = await moduleResponse.Content.ReadFromJsonAsync<CreateModuleCommandResponse>();

        var createUseCaseRequest = new { Title = "Test UseCase", Description = "Test Description", ImportantNotes = (string?)null };
        var useCaseResponse = await Client.PostAsJsonAsync($"/api/v1/usecases/modules/{module!.Id}", createUseCaseRequest);
        useCaseResponse.EnsureSuccessStatusCode();
        var useCase = await useCaseResponse.Content.ReadFromJsonAsync<CreateUseCaseCommandResponse>();

        // Act
        var response = await Client.DeleteAsync($"/api/v1/usecases/{useCase!.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteUseCase_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var token = await RegisterAndGetTokenAsync();
        SetAuthorizationHeader(token);
        var invalidId = Guid.NewGuid();

        // Act
        var response = await Client.DeleteAsync($"/api/v1/usecases/{invalidId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}

