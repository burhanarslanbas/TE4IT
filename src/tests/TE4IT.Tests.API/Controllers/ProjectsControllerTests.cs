using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using TE4IT.API;
using TE4IT.Application.Common.Pagination;
using TE4IT.Application.Features.Auth.Commands.Register;
using TE4IT.Application.Features.Projects.Commands.CreateProject;
using TE4IT.Application.Features.Projects.Queries.GetProjectById;
using TE4IT.Application.Features.Projects.Queries.ListProjects;
using TE4IT.Application.Features.Projects.Responses;
using TE4IT.Domain.Enums;
using TE4IT.Domain.Services;
using TE4IT.Tests.API.Common;
using Xunit;

namespace TE4IT.Tests.API.Controllers;

public class ProjectsControllerTests : ApiTestBase
{
    public ProjectsControllerTests(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    [Fact]
    public async Task CreateProject_WithValidRequest_ReturnsCreated()
    {
        // Arrange
        var token = await RegisterAndGetTokenAsync();
        SetAuthorizationHeader(token);

        var request = new CreateProjectCommand("Test Project", "Test Description");

        // Act
        var response = await Client.PostAsJsonAsync("/api/v1/projects", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var result = await response.Content.ReadFromJsonAsync<CreateProjectCommandResponse>();
        result.Should().NotBeNull();
        result!.Id.Should().NotBeEmpty();
    }

    [Fact]
    public async Task CreateProject_WithoutAuth_ReturnsUnauthorized()
    {
        // Arrange
        ClearAuthorizationHeader();
        var request = new CreateProjectCommand("Test Project", "Test Description");

        // Act
        var response = await Client.PostAsJsonAsync("/api/v1/projects", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetProjectById_WithValidId_ReturnsProject()
    {
        // Arrange
        var token = await RegisterAndGetTokenAsync();
        SetAuthorizationHeader(token);

        // Önce bir proje oluştur
        var createRequest = new CreateProjectCommand("Test Project", "Test Description");
        var createResponse = await Client.PostAsJsonAsync("/api/v1/projects", createRequest);
        createResponse.EnsureSuccessStatusCode();
        var createdProject = await createResponse.Content.ReadFromJsonAsync<CreateProjectCommandResponse>();

        // Act
        var response = await Client.GetAsync($"/api/v1/projects/{createdProject!.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<ProjectResponse>();
        result.Should().NotBeNull();
        result!.Id.Should().Be(createdProject.Id);
        result.Title.Should().Be("Test Project");
    }

    [Fact]
    public async Task GetProjectById_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var token = await RegisterAndGetTokenAsync();
        SetAuthorizationHeader(token);
        var invalidId = Guid.NewGuid();

        // Act
        var response = await Client.GetAsync($"/api/v1/projects/{invalidId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ListProjects_ReturnsPagedResults()
    {
        // Arrange
        var token = await RegisterAndGetTokenAsync();
        SetAuthorizationHeader(token);

        // Trial kullanıcı sadece 1 proje oluşturabilir, bu yüzden 1 proje oluştur
        var createRequest = new CreateProjectCommand("Test Project", "Test Description");
        var createResponse = await Client.PostAsJsonAsync("/api/v1/projects", createRequest);
        createResponse.EnsureSuccessStatusCode();

        // Act
        var response = await Client.GetAsync("/api/v1/projects?page=1&pageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PagedResult<ProjectListItemResponse>>();
        result.Should().NotBeNull();
        result!.Items.Should().HaveCountGreaterThanOrEqualTo(1);
    }

    [Fact]
    public async Task ListProjectMembers_WithValidProject_ReturnsMembers()
    {
        // Arrange
        var ownerToken = await RegisterAndGetTokenAsync("owner2@example.com", "Test123!@#", "owner2");
        SetAuthorizationHeader(ownerToken);

        // Proje oluştur
        var createRequest = new CreateProjectCommand("Test Project", "Test Description");
        var createResponse = await Client.PostAsJsonAsync("/api/v1/projects", createRequest);
        createResponse.EnsureSuccessStatusCode();
        var project = await createResponse.Content.ReadFromJsonAsync<CreateProjectCommandResponse>();

        // Act
        var response = await Client.GetAsync($"/api/v1/projects/{project!.Id}/members");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<List<ProjectMemberResponse>>();
        result.Should().NotBeNull();
        result!.Should().HaveCountGreaterThanOrEqualTo(1); // En az owner olmalı
    }

    [Fact]
    public async Task UpdateProject_WithValidRequest_ReturnsNoContent()
    {
        // Arrange
        var token = await RegisterAndGetTokenAsync();
        SetAuthorizationHeader(token);

        // Proje oluştur
        var createRequest = new CreateProjectCommand("Test Project", "Test Description");
        var createResponse = await Client.PostAsJsonAsync("/api/v1/projects", createRequest);
        createResponse.EnsureSuccessStatusCode();
        var project = await createResponse.Content.ReadFromJsonAsync<CreateProjectCommandResponse>();

        var updateRequest = new { Title = "Updated Project", Description = "Updated Description" };

        // Act
        var response = await Client.PutAsJsonAsync($"/api/v1/projects/{project!.Id}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task UpdateProject_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var token = await RegisterAndGetTokenAsync();
        SetAuthorizationHeader(token);
        var invalidId = Guid.NewGuid();

        var updateRequest = new { Title = "Updated Project", Description = "Updated Description" };

        // Act
        var response = await Client.PutAsJsonAsync($"/api/v1/projects/{invalidId}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ChangeProjectStatus_WithValidRequest_ReturnsNoContent()
    {
        // Arrange
        var token = await RegisterAndGetTokenAsync();
        SetAuthorizationHeader(token);

        // Proje oluştur
        var createRequest = new CreateProjectCommand("Test Project", "Test Description");
        var createResponse = await Client.PostAsJsonAsync("/api/v1/projects", createRequest);
        createResponse.EnsureSuccessStatusCode();
        var project = await createResponse.Content.ReadFromJsonAsync<CreateProjectCommandResponse>();

        var changeStatusRequest = new { IsActive = false };

        // Act
        var response = await Client.PatchAsJsonAsync($"/api/v1/projects/{project!.Id}/status", changeStatusRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task ChangeProjectStatus_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var token = await RegisterAndGetTokenAsync();
        SetAuthorizationHeader(token);
        var invalidId = Guid.NewGuid();

        var changeStatusRequest = new { IsActive = false };

        // Act
        var response = await Client.PatchAsJsonAsync($"/api/v1/projects/{invalidId}/status", changeStatusRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteProject_WithValidId_ReturnsNoContent()
    {
        // Arrange
        var token = await RegisterAndGetTokenAsync();
        SetAuthorizationHeader(token);

        // Proje oluştur
        var createRequest = new CreateProjectCommand("Test Project", "Test Description");
        var createResponse = await Client.PostAsJsonAsync("/api/v1/projects", createRequest);
        createResponse.EnsureSuccessStatusCode();
        var project = await createResponse.Content.ReadFromJsonAsync<CreateProjectCommandResponse>();

        // Act
        var response = await Client.DeleteAsync($"/api/v1/projects/{project!.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteProject_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var token = await RegisterAndGetTokenAsync();
        SetAuthorizationHeader(token);
        var invalidId = Guid.NewGuid();

        // Act
        var response = await Client.DeleteAsync($"/api/v1/projects/{invalidId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task RemoveProjectMember_WithValidRequest_ReturnsNoContent()
    {
        // Arrange
        var ownerToken = await RegisterAndGetTokenAsync("owner3@example.com", "Test123!@#", "owner3");
        SetAuthorizationHeader(ownerToken);

        // Proje oluştur
        var createRequest = new CreateProjectCommand("Test Project", "Test Description");
        var createResponse = await Client.PostAsJsonAsync("/api/v1/projects", createRequest);
        createResponse.EnsureSuccessStatusCode();
        var project = await createResponse.Content.ReadFromJsonAsync<CreateProjectCommandResponse>();

        // Member ekle
        var memberUserId = await GetUserIdFromTokenAsync(ownerToken);
        var addMemberRequest = new { UserId = memberUserId, Role = ProjectRole.Member };
        var addMemberResponse = await Client.PostAsJsonAsync($"/api/v1/projects/{project!.Id}/members", addMemberRequest);
        addMemberResponse.EnsureSuccessStatusCode();

        // Act - Member'ı çıkar
        var response = await Client.DeleteAsync($"/api/v1/projects/{project.Id}/members/{memberUserId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task RemoveProjectMember_WithInvalidProjectId_ReturnsNotFound()
    {
        // Arrange
        var ownerToken = await RegisterAndGetTokenAsync("owner4@example.com", "Test123!@#", "owner4");
        SetAuthorizationHeader(ownerToken);

        var invalidProjectId = Guid.NewGuid();
        var memberUserId = await GetUserIdFromTokenAsync(ownerToken);

        // Act
        var response = await Client.DeleteAsync($"/api/v1/projects/{invalidProjectId}/members/{memberUserId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateProjectMemberRole_WithValidRequest_ReturnsNoContent()
    {
        // Arrange
        var ownerToken = await RegisterAndGetTokenAsync("owner5@example.com", "Test123!@#", "owner5");
        SetAuthorizationHeader(ownerToken);

        // Proje oluştur
        var createRequest = new CreateProjectCommand("Test Project", "Test Description");
        var createResponse = await Client.PostAsJsonAsync("/api/v1/projects", createRequest);
        createResponse.EnsureSuccessStatusCode();
        var project = await createResponse.Content.ReadFromJsonAsync<CreateProjectCommandResponse>();

        // Member ekle
        var memberUserId = await GetUserIdFromTokenAsync(ownerToken);
        var addMemberRequest = new { UserId = memberUserId, Role = ProjectRole.Member };
        var addMemberResponse = await Client.PostAsJsonAsync($"/api/v1/projects/{project!.Id}/members", addMemberRequest);
        addMemberResponse.EnsureSuccessStatusCode();

        // Act - Member rolünü Viewer'a güncelle
        var updateRoleRequest = new { Role = ProjectRole.Viewer };
        var response = await Client.PutAsJsonAsync($"/api/v1/projects/{project.Id}/members/{memberUserId}/role", updateRoleRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task UpdateProjectMemberRole_WithInvalidProjectId_ReturnsNotFound()
    {
        // Arrange
        var ownerToken = await RegisterAndGetTokenAsync("owner6@example.com", "Test123!@#", "owner6");
        SetAuthorizationHeader(ownerToken);

        var invalidProjectId = Guid.NewGuid();
        var memberUserId = await GetUserIdFromTokenAsync(ownerToken);

        var updateRoleRequest = new { Role = ProjectRole.Viewer };

        // Act
        var response = await Client.PutAsJsonAsync($"/api/v1/projects/{invalidProjectId}/members/{memberUserId}/role", updateRoleRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private async Task<Guid> GetUserIdFromTokenAsync(string token)
    {
        // Token'dan user ID'yi almak için login response'u kullan
        // Register işlemi sonrası user ID'yi saklamak daha iyi olur
        // Şimdilik test için basit bir yaklaşım: yeni bir kullanıcı oluştur ve register response'dan al
        var registerResponse = await Client.PostAsJsonAsync("/api/v1/auth/register", 
            new RegisterCommand("testmember", $"member{Guid.NewGuid()}@example.com", "Test123!@#"));
        registerResponse.EnsureSuccessStatusCode();
        var registerResult = await registerResponse.Content.ReadFromJsonAsync<RegisterCommandResponse>();
        return registerResult!.UserId;
    }
}


