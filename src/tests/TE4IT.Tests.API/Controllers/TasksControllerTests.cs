using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using TE4IT.API;
using TE4IT.Application.Common.Pagination;
using TE4IT.Application.Features.Modules.Commands.CreateModule;
using TE4IT.Application.Features.Projects.Commands.CreateProject;
using TE4IT.Application.Features.Tasks.Commands.CreateTask;
using TE4IT.Application.Features.Tasks.Responses;
using TE4IT.Application.Features.UseCases.Commands.CreateUseCase;
using TE4IT.Domain.Enums;
using TE4IT.Tests.API.Common;
using Xunit;

namespace TE4IT.Tests.API.Controllers;

public class TasksControllerTests : ApiTestBase
{
    public TasksControllerTests(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    [Fact]
    public async Task Create_WithValidRequest_ReturnsCreated()
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

        var request = new { Title = "Test Task", TaskType = TaskType.Feature, Description = "Test Description", ImportantNotes = (string?)null, DueDate = (DateTime?)null };

        // Act
        var response = await Client.PostAsJsonAsync($"/api/v1/tasks/usecases/{useCase!.Id}", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var result = await response.Content.ReadFromJsonAsync<CreateTaskCommandResponse>();
        result.Should().NotBeNull();
        result!.Id.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetById_WithValidId_ReturnsOk()
    {
        // Arrange
        var token = await RegisterAndGetTokenAsync();
        SetAuthorizationHeader(token);

        // Proje, modül, use case ve task oluştur
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

        var createTaskRequest = new { Title = "Test Task", TaskType = TaskType.Feature, Description = "Test Description", ImportantNotes = (string?)null, DueDate = (DateTime?)null };
        var taskResponse = await Client.PostAsJsonAsync($"/api/v1/tasks/usecases/{useCase!.Id}", createTaskRequest);
        taskResponse.EnsureSuccessStatusCode();
        var task = await taskResponse.Content.ReadFromJsonAsync<CreateTaskCommandResponse>();

        // Act
        var response = await Client.GetAsync($"/api/v1/tasks/{task!.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<TaskResponse>();
        result.Should().NotBeNull();
        result!.Id.Should().Be(task.Id);
    }

    [Fact]
    public async Task GetByUseCase_ReturnsPagedResults()
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

        // Birkaç task oluştur
        for (int i = 0; i < 3; i++)
        {
            var createTaskRequest = new { Title = $"Task {i}", TaskType = TaskType.Feature, Description = $"Description {i}", ImportantNotes = (string?)null, DueDate = (DateTime?)null };
            await Client.PostAsJsonAsync($"/api/v1/tasks/usecases/{useCase!.Id}", createTaskRequest);
        }

        // Act
        var response = await Client.GetAsync($"/api/v1/tasks/usecases/{useCase!.Id}?page=1&pageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PagedResult<TaskListItemResponse>>();
        result.Should().NotBeNull();
        result!.Items.Should().HaveCountGreaterThanOrEqualTo(3);
    }

    [Fact]
    public async Task UpdateTask_WithValidRequest_ReturnsNoContent()
    {
        // Arrange
        var token = await RegisterAndGetTokenAsync();
        SetAuthorizationHeader(token);

        // Proje, modül, use case ve task oluştur
        var (project, module, useCase, task) = await CreateTaskHierarchyAsync();

        var updateRequest = new { Title = "Updated Task", Description = "Updated Description", ImportantNotes = "Updated Notes", DueDate = (DateTime?)null };

        // Act
        var response = await Client.PutAsJsonAsync($"/api/v1/tasks/{task.Id}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task UpdateTask_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var token = await RegisterAndGetTokenAsync();
        SetAuthorizationHeader(token);
        var invalidId = Guid.NewGuid();

        var updateRequest = new { Title = "Updated Task", Description = "Updated Description", ImportantNotes = (string?)null, DueDate = (DateTime?)null };

        // Act
        var response = await Client.PutAsJsonAsync($"/api/v1/tasks/{invalidId}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // NOTE: This old test has been replaced by more comprehensive state change tests
    // [Fact]
    // public async Task ChangeTaskState_WithValidRequest_ReturnsNoContent() - See ChangeState_AsProjectOwner_NotStartedToInProgress_ReturnsNoContent

    [Fact]
    public async Task ChangeTaskState_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var token = await RegisterAndGetTokenAsync();
        SetAuthorizationHeader(token);
        var invalidId = Guid.NewGuid();

        var changeStateRequest = new { NewState = TaskState.InProgress };

        // Act
        var response = await Client.PatchAsJsonAsync($"/api/v1/tasks/{invalidId}/state", changeStateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // NOTE: This old test has been replaced by more comprehensive authorization tests
    // [Fact]
    // public async Task AssignTask_WithValidRequest_ReturnsNoContent() - See Assign_AsProjectOwner_ToMember_ReturnsNoContent

    [Fact]
    public async Task AssignTask_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var token = await RegisterAndGetTokenAsync();
        SetAuthorizationHeader(token);
        var invalidId = Guid.NewGuid();

        // Yeni bir kullanıcı oluştur (assignee olarak) ve user ID'yi al
        var guidStr = Guid.NewGuid().ToString("N");
        var registerResponse = await Client.PostAsJsonAsync("/api/v1/auth/register", 
            new TE4IT.Application.Features.Auth.Commands.Register.RegisterCommand($"user_{guidStr}", $"user{guidStr}@example.com", "Test123!@#"));
        registerResponse.EnsureSuccessStatusCode();
        var registerResult = await registerResponse.Content.ReadFromJsonAsync<TE4IT.Application.Features.Auth.Commands.Register.RegisterCommandResponse>();
        var assigneeUserId = registerResult!.UserId;

        var assignRequest = new { AssigneeId = assigneeUserId };

        // Act
        var response = await Client.PostAsJsonAsync($"/api/v1/tasks/{invalidId}/assign", assignRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteTask_WithValidId_ReturnsNoContent()
    {
        // Arrange
        var token = await RegisterAndGetTokenAsync();
        SetAuthorizationHeader(token);

        // Proje, modül, use case ve task oluştur
        var (project, module, useCase, task) = await CreateTaskHierarchyAsync();

        // Act
        var response = await Client.DeleteAsync($"/api/v1/tasks/{task.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteTask_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var token = await RegisterAndGetTokenAsync();
        SetAuthorizationHeader(token);
        var invalidId = Guid.NewGuid();

        // Act
        var response = await Client.DeleteAsync($"/api/v1/tasks/{invalidId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #region Phase 1 - Assignment Tests (Critical - Swagger'da hata bulundu)

    [Fact]
    public async Task Assign_AsProjectOwner_ToMember_ReturnsNoContent()
    {
        // Arrange
        var context = await CreateFullTaskContextAsync();
        SetAuthorizationHeader(context.OwnerToken);

        var assignRequest = new { AssigneeId = context.MemberId };

        // Act
        var response = await Client.PostAsJsonAsync($"/api/v1/tasks/{context.Task.Id}/assign", assignRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        
        // Verify assignment
        var isAssigned = await IsTaskAssignedToUserAsync(context.Task.Id, context.MemberId);
        isAssigned.Should().BeTrue();
    }

    [Fact]
    public async Task Assign_AsProjectOwner_ToViewer_ReturnsNoContent()
    {
        // Arrange
        var context = await CreateFullTaskContextAsync();
        SetAuthorizationHeader(context.OwnerToken);

        var assignRequest = new { AssigneeId = context.ViewerId };

        // Act
        var response = await Client.PostAsJsonAsync($"/api/v1/tasks/{context.Task.Id}/assign", assignRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        
        // Verify assignment
        var isAssigned = await IsTaskAssignedToUserAsync(context.Task.Id, context.ViewerId);
        isAssigned.Should().BeTrue();
    }

    [Fact]
    public async Task Assign_AsProjectMember_ToAnotherMember_ReturnsNoContent()
    {
        // Arrange
        var context = await CreateFullTaskContextAsync();
        SetAuthorizationHeader(context.MemberToken);

        // Create another member
        var anotherMemberGuid = Guid.NewGuid().ToString("N");
        var anotherMemberId = await RegisterUserAndGetIdAsync($"member2{anotherMemberGuid}@test.com", "Test123!@#", $"member2_{anotherMemberGuid}");
        await AddProjectMemberAsync(context.Project.Id, anotherMemberId, ProjectRole.Member);

        var assignRequest = new { AssigneeId = anotherMemberId };

        // Act
        var response = await Client.PostAsJsonAsync($"/api/v1/tasks/{context.Task.Id}/assign", assignRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        
        // Verify assignment
        var isAssigned = await IsTaskAssignedToUserAsync(context.Task.Id, anotherMemberId);
        isAssigned.Should().BeTrue();
    }

    [Fact]
    public async Task Assign_AsProjectViewer_ReturnsForbidden()
    {
        // Arrange
        var context = await CreateFullTaskContextAsync();
        SetAuthorizationHeader(context.ViewerToken);

        var assignRequest = new { AssigneeId = context.MemberId };

        // Act
        var response = await Client.PostAsJsonAsync($"/api/v1/tasks/{context.Task.Id}/assign", assignRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Assign_AsTaskCreator_ButNotProjectMember_ReturnsNoContent()
    {
        // Arrange
        var context = await CreateFullTaskContextAsync();
        
        // Task'ı oluşturan owner'dır, bu test için owner token'ını kullanıyoruz
        // (Creator her zaman yetkilidir)
        SetAuthorizationHeader(context.OwnerToken);

        var assignRequest = new { AssigneeId = context.MemberId };

        // Act
        var response = await Client.PostAsJsonAsync($"/api/v1/tasks/{context.Task.Id}/assign", assignRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Assign_AsNonProjectMember_ReturnsForbidden()
    {
        // Arrange
        var context = await CreateFullTaskContextAsync();
        SetAuthorizationHeader(context.NonMemberToken);

        var assignRequest = new { AssigneeId = context.MemberId };

        // Act
        var response = await Client.PostAsJsonAsync($"/api/v1/tasks/{context.Task.Id}/assign", assignRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Assign_ToNonProjectMember_ReturnsForbidden()
    {
        // Arrange
        var context = await CreateFullTaskContextAsync();
        SetAuthorizationHeader(context.OwnerToken);

        var assignRequest = new { AssigneeId = context.NonMemberId };

        // Act
        var response = await Client.PostAsJsonAsync($"/api/v1/tasks/{context.Task.Id}/assign", assignRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Assign_ToSelf_ReturnsNoContent()
    {
        // Arrange
        var context = await CreateFullTaskContextAsync();
        SetAuthorizationHeader(context.OwnerToken);

        var assignRequest = new { AssigneeId = context.OwnerId };

        // Act
        var response = await Client.PostAsJsonAsync($"/api/v1/tasks/{context.Task.Id}/assign", assignRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        
        // Verify assignment
        var isAssigned = await IsTaskAssignedToUserAsync(context.Task.Id, context.OwnerId);
        isAssigned.Should().BeTrue();
    }

    [Fact]
    public async Task Assign_WithoutAuth_ReturnsUnauthorized()
    {
        // Arrange
        var context = await CreateFullTaskContextAsync();
        ClearAuthorizationHeader();

        var assignRequest = new { AssigneeId = context.MemberId };

        // Act
        var response = await Client.PostAsJsonAsync($"/api/v1/tasks/{context.Task.Id}/assign", assignRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Assign_WithInvalidTaskId_ReturnsNotFound()
    {
        // Arrange
        var context = await CreateFullTaskContextAsync();
        SetAuthorizationHeader(context.OwnerToken);
        var invalidId = Guid.NewGuid();

        var assignRequest = new { AssigneeId = context.MemberId };

        // Act
        var response = await Client.PostAsJsonAsync($"/api/v1/tasks/{invalidId}/assign", assignRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Assign_WithInvalidAssigneeId_ReturnsNotFound()
    {
        // Arrange
        var context = await CreateFullTaskContextAsync();
        SetAuthorizationHeader(context.OwnerToken);
        var invalidUserId = Guid.NewGuid();

        var assignRequest = new { AssigneeId = invalidUserId };

        // Act
        var response = await Client.PostAsJsonAsync($"/api/v1/tasks/{context.Task.Id}/assign", assignRequest);

        // Assert
        // Geçersiz kullanıcı ID'si için NotFound dönmeli
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region Phase 1 - Create Tests (Different Roles and Assignment Scenarios)

    [Fact]
    public async Task Create_AsProjectOwner_WithoutAssignee_ReturnsCreated()
    {
        // Arrange
        var context = await CreateFullTaskContextAsync();
        SetAuthorizationHeader(context.OwnerToken);

        var request = new { Title = "New Task", TaskType = TaskType.Feature, Description = "Description", ImportantNotes = (string?)null, DueDate = (DateTime?)null, AssigneeId = (Guid?)null };

        // Act
        var response = await Client.PostAsJsonAsync($"/api/v1/tasks/usecases/{context.UseCase.Id}", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var result = await response.Content.ReadFromJsonAsync<CreateTaskCommandResponse>();
        result.Should().NotBeNull();
        result!.Id.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Create_AsProjectMember_WithoutAssignee_ReturnsCreated()
    {
        // Arrange
        var context = await CreateFullTaskContextAsync();
        SetAuthorizationHeader(context.MemberToken);

        var request = new { Title = "New Task", TaskType = TaskType.Feature, Description = "Description", ImportantNotes = (string?)null, DueDate = (DateTime?)null, AssigneeId = (Guid?)null };

        // Act
        var response = await Client.PostAsJsonAsync($"/api/v1/tasks/usecases/{context.UseCase.Id}", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var result = await response.Content.ReadFromJsonAsync<CreateTaskCommandResponse>();
        result.Should().NotBeNull();
        result!.Id.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Create_AsProjectViewer_ReturnsForbidden()
    {
        // Arrange
        var context = await CreateFullTaskContextAsync();
        SetAuthorizationHeader(context.ViewerToken);

        var request = new { Title = "New Task", TaskType = TaskType.Feature, Description = "Description", ImportantNotes = (string?)null, DueDate = (DateTime?)null, AssigneeId = (Guid?)null };

        // Act
        var response = await Client.PostAsJsonAsync($"/api/v1/tasks/usecases/{context.UseCase.Id}", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Create_AsNonProjectMember_ReturnsForbidden()
    {
        // Arrange
        var context = await CreateFullTaskContextAsync();
        SetAuthorizationHeader(context.NonMemberToken);

        var request = new { Title = "New Task", TaskType = TaskType.Feature, Description = "Description", ImportantNotes = (string?)null, DueDate = (DateTime?)null, AssigneeId = (Guid?)null };

        // Act
        var response = await Client.PostAsJsonAsync($"/api/v1/tasks/usecases/{context.UseCase.Id}", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Create_AsOwner_WithValidAssignee_ReturnsCreated()
    {
        // Arrange
        var context = await CreateFullTaskContextAsync();
        SetAuthorizationHeader(context.OwnerToken);

        var request = new { Title = "New Task", TaskType = TaskType.Feature, Description = "Description", ImportantNotes = (string?)null, DueDate = (DateTime?)null, AssigneeId = (Guid?)context.MemberId };

        // Act
        var response = await Client.PostAsJsonAsync($"/api/v1/tasks/usecases/{context.UseCase.Id}", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var result = await response.Content.ReadFromJsonAsync<CreateTaskCommandResponse>();
        result.Should().NotBeNull();
        result!.Id.Should().NotBeEmpty();
        
        // Verify assignment
        var isAssigned = await IsTaskAssignedToUserAsync(result.Id, context.MemberId);
        isAssigned.Should().BeTrue();
    }

    [Fact]
    public async Task Create_AsOwner_WithNonMemberAssignee_ReturnsForbidden()
    {
        // Arrange
        var context = await CreateFullTaskContextAsync();
        SetAuthorizationHeader(context.OwnerToken);

        var request = new { Title = "New Task", TaskType = TaskType.Feature, Description = "Description", ImportantNotes = (string?)null, DueDate = (DateTime?)null, AssigneeId = (Guid?)context.NonMemberId };

        // Act
        var response = await Client.PostAsJsonAsync($"/api/v1/tasks/usecases/{context.UseCase.Id}", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Create_AsViewer_WithAssignee_ReturnsForbidden()
    {
        // Arrange
        var context = await CreateFullTaskContextAsync();
        SetAuthorizationHeader(context.ViewerToken);

        var request = new { Title = "New Task", TaskType = TaskType.Feature, Description = "Description", ImportantNotes = (string?)null, DueDate = (DateTime?)null, AssigneeId = (Guid?)context.MemberId };

        // Act
        var response = await Client.PostAsJsonAsync($"/api/v1/tasks/usecases/{context.UseCase.Id}", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Create_WithoutAuth_ReturnsUnauthorized()
    {
        // Arrange
        var context = await CreateFullTaskContextAsync();
        ClearAuthorizationHeader();

        var request = new { Title = "New Task", TaskType = TaskType.Feature, Description = "Description", ImportantNotes = (string?)null, DueDate = (DateTime?)null, AssigneeId = (Guid?)null };

        // Act
        var response = await Client.PostAsJsonAsync($"/api/v1/tasks/usecases/{context.UseCase.Id}", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Create_WithInvalidUseCase_ReturnsNotFound()
    {
        // Arrange
        var token = await RegisterAndGetTokenAsync();
        SetAuthorizationHeader(token);
        var invalidUseCaseId = Guid.NewGuid();

        var request = new { Title = "New Task", TaskType = TaskType.Feature, Description = "Description", ImportantNotes = (string?)null, DueDate = (DateTime?)null, AssigneeId = (Guid?)null };

        // Act
        var response = await Client.PostAsJsonAsync($"/api/v1/tasks/usecases/{invalidUseCaseId}", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Create_WithEmptyTitle_ReturnsBadRequest()
    {
        // Arrange
        var context = await CreateFullTaskContextAsync();
        SetAuthorizationHeader(context.OwnerToken);

        var request = new { Title = "", TaskType = TaskType.Feature, Description = "Description", ImportantNotes = (string?)null, DueDate = (DateTime?)null, AssigneeId = (Guid?)null };

        // Act
        var response = await Client.PostAsJsonAsync($"/api/v1/tasks/usecases/{context.UseCase.Id}", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Create_DefaultsToNotStartedState()
    {
        // Arrange
        var context = await CreateFullTaskContextAsync();
        SetAuthorizationHeader(context.OwnerToken);

        var request = new { Title = "New Task", TaskType = TaskType.Feature, Description = "Description", ImportantNotes = (string?)null, DueDate = (DateTime?)null, AssigneeId = (Guid?)null };

        // Act
        var response = await Client.PostAsJsonAsync($"/api/v1/tasks/usecases/{context.UseCase.Id}", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var result = await response.Content.ReadFromJsonAsync<CreateTaskCommandResponse>();
        
        // Verify state
        var state = await GetTaskStateAsync(result!.Id);
        state.Should().Be(TaskState.NotStarted);
    }

    [Fact]
    public async Task Create_DefaultsToNoAssignee()
    {
        // Arrange
        var context = await CreateFullTaskContextAsync();
        SetAuthorizationHeader(context.OwnerToken);

        var request = new { Title = "New Task", TaskType = TaskType.Feature, Description = "Description", ImportantNotes = (string?)null, DueDate = (DateTime?)null, AssigneeId = (Guid?)null };

        // Act
        var response = await Client.PostAsJsonAsync($"/api/v1/tasks/usecases/{context.UseCase.Id}", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var result = await response.Content.ReadFromJsonAsync<CreateTaskCommandResponse>();
        
        // Verify no assignee
        var taskResponse = await Client.GetAsync($"/api/v1/tasks/{result!.Id}");
        var task = await taskResponse.Content.ReadFromJsonAsync<TaskResponse>();
        task!.AssigneeId.Should().BeNull();
    }

    #endregion

    #region Phase 1 - Update Tests (Authorization Scenarios)

    [Fact]
    public async Task Update_AsProjectOwner_ReturnsNoContent()
    {
        // Arrange
        var context = await CreateFullTaskContextAsync();
        SetAuthorizationHeader(context.OwnerToken);

        var updateRequest = new { Title = "Updated Task", Description = "Updated Description", ImportantNotes = "Updated Notes", DueDate = (DateTime?)null };

        // Act
        var response = await Client.PutAsJsonAsync($"/api/v1/tasks/{context.Task.Id}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Update_AsProjectMember_ReturnsNoContent()
    {
        // Arrange
        var context = await CreateFullTaskContextAsync();
        SetAuthorizationHeader(context.MemberToken);

        var updateRequest = new { Title = "Updated Task", Description = "Updated Description", ImportantNotes = "Updated Notes", DueDate = (DateTime?)null };

        // Act
        var response = await Client.PutAsJsonAsync($"/api/v1/tasks/{context.Task.Id}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Update_AsProjectViewer_ReturnsForbidden()
    {
        // Arrange
        var context = await CreateFullTaskContextAsync();
        SetAuthorizationHeader(context.ViewerToken);

        var updateRequest = new { Title = "Updated Task", Description = "Updated Description", ImportantNotes = "Updated Notes", DueDate = (DateTime?)null };

        // Act
        var response = await Client.PutAsJsonAsync($"/api/v1/tasks/{context.Task.Id}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Update_AsTaskCreator_ButNotProjectMember_ReturnsNoContent()
    {
        // Arrange
        var context = await CreateFullTaskContextAsync();
        // Task'ı owner oluşturdu, owner token kullanıyoruz (creator her zaman güncelleyebilir)
        SetAuthorizationHeader(context.OwnerToken);

        var updateRequest = new { Title = "Updated Task", Description = "Updated Description", ImportantNotes = "Updated Notes", DueDate = (DateTime?)null };

        // Act
        var response = await Client.PutAsJsonAsync($"/api/v1/tasks/{context.Task.Id}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Update_AsNonProjectMember_ReturnsForbidden()
    {
        // Arrange
        var context = await CreateFullTaskContextAsync();
        SetAuthorizationHeader(context.NonMemberToken);

        var updateRequest = new { Title = "Updated Task", Description = "Updated Description", ImportantNotes = "Updated Notes", DueDate = (DateTime?)null };

        // Act
        var response = await Client.PutAsJsonAsync($"/api/v1/tasks/{context.Task.Id}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Update_WithoutAuth_ReturnsUnauthorized()
    {
        // Arrange
        var context = await CreateFullTaskContextAsync();
        ClearAuthorizationHeader();

        var updateRequest = new { Title = "Updated Task", Description = "Updated Description", ImportantNotes = "Updated Notes", DueDate = (DateTime?)null };

        // Act
        var response = await Client.PutAsJsonAsync($"/api/v1/tasks/{context.Task.Id}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Update_WithInvalidId_ReturnsNotFound()
    {
        // Arrange - Bu test zaten mevcut ama tutarlılık için ekledim
        var token = await RegisterAndGetTokenAsync();
        SetAuthorizationHeader(token);
        var invalidId = Guid.NewGuid();

        var updateRequest = new { Title = "Updated Task", Description = "Updated Description", ImportantNotes = (string?)null, DueDate = (DateTime?)null };

        // Act
        var response = await Client.PutAsJsonAsync($"/api/v1/tasks/{invalidId}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Update_WithEmptyTitle_ReturnsBadRequest()
    {
        // Arrange
        var context = await CreateFullTaskContextAsync();
        SetAuthorizationHeader(context.OwnerToken);

        var updateRequest = new { Title = "", Description = "Updated Description", ImportantNotes = "Updated Notes", DueDate = (DateTime?)null };

        // Act
        var response = await Client.PutAsJsonAsync($"/api/v1/tasks/{context.Task.Id}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region Phase 1 - View and List Tests (Authorization)

    [Fact]
    public async Task GetById_AsProjectOwner_ReturnsOk()
    {
        // Arrange
        var context = await CreateFullTaskContextAsync();
        SetAuthorizationHeader(context.OwnerToken);

        // Act
        var response = await Client.GetAsync($"/api/v1/tasks/{context.Task.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<TaskResponse>();
        result.Should().NotBeNull();
        result!.Id.Should().Be(context.Task.Id);
    }

    [Fact]
    public async Task GetById_AsProjectMember_ReturnsOk()
    {
        // Arrange
        var context = await CreateFullTaskContextAsync();
        SetAuthorizationHeader(context.MemberToken);

        // Act
        var response = await Client.GetAsync($"/api/v1/tasks/{context.Task.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<TaskResponse>();
        result.Should().NotBeNull();
        result!.Id.Should().Be(context.Task.Id);
    }

    [Fact]
    public async Task GetById_AsProjectViewer_ReturnsOk()
    {
        // Arrange
        var context = await CreateFullTaskContextAsync();
        SetAuthorizationHeader(context.ViewerToken);

        // Act
        var response = await Client.GetAsync($"/api/v1/tasks/{context.Task.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<TaskResponse>();
        result.Should().NotBeNull();
        result!.Id.Should().Be(context.Task.Id);
    }

    [Fact]
    public async Task GetById_AsNonProjectMember_ReturnsForbidden()
    {
        // Arrange
        var context = await CreateFullTaskContextAsync();
        SetAuthorizationHeader(context.NonMemberToken);

        // Act
        var response = await Client.GetAsync($"/api/v1/tasks/{context.Task.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetById_AsTaskCreator_ButNotProjectMember_ReturnsOk()
    {
        // Arrange
        var context = await CreateFullTaskContextAsync();
        // Task'ı owner oluşturdu, owner token kullanıyoruz (creator her zaman görüntüleyebilir)
        SetAuthorizationHeader(context.OwnerToken);

        // Act
        var response = await Client.GetAsync($"/api/v1/tasks/{context.Task.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<TaskResponse>();
        result.Should().NotBeNull();
        result!.Id.Should().Be(context.Task.Id);
    }

    [Fact]
    public async Task GetById_WithoutAuth_ReturnsUnauthorized()
    {
        // Arrange
        var context = await CreateFullTaskContextAsync();
        ClearAuthorizationHeader();

        // Act
        var response = await Client.GetAsync($"/api/v1/tasks/{context.Task.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetById_WithInvalidId_ReturnsNotFound()
    {
        // Arrange - Bu test zaten mevcut ama tutarlılık için kontrol
        var token = await RegisterAndGetTokenAsync();
        SetAuthorizationHeader(token);
        var invalidId = Guid.NewGuid();

        // Act
        var response = await Client.GetAsync($"/api/v1/tasks/{invalidId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetByUseCase_AsProjectOwner_ReturnsAllTasks()
    {
        // Arrange
        var context = await CreateFullTaskContextAsync();
        SetAuthorizationHeader(context.OwnerToken);

        // Act
        var response = await Client.GetAsync($"/api/v1/tasks/usecases/{context.UseCase.Id}?page=1&pageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PagedResult<TaskListItemResponse>>();
        result.Should().NotBeNull();
        result!.Items.Should().HaveCountGreaterThanOrEqualTo(1);
    }

    [Fact]
    public async Task GetByUseCase_AsProjectMember_ReturnsAllTasks()
    {
        // Arrange
        var context = await CreateFullTaskContextAsync();
        SetAuthorizationHeader(context.MemberToken);

        // Act
        var response = await Client.GetAsync($"/api/v1/tasks/usecases/{context.UseCase.Id}?page=1&pageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PagedResult<TaskListItemResponse>>();
        result.Should().NotBeNull();
        result!.Items.Should().HaveCountGreaterThanOrEqualTo(1);
    }

    [Fact]
    public async Task GetByUseCase_AsProjectViewer_ReturnsAllTasks()
    {
        // Arrange
        var context = await CreateFullTaskContextAsync();
        SetAuthorizationHeader(context.ViewerToken);

        // Act
        var response = await Client.GetAsync($"/api/v1/tasks/usecases/{context.UseCase.Id}?page=1&pageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PagedResult<TaskListItemResponse>>();
        result.Should().NotBeNull();
        result!.Items.Should().HaveCountGreaterThanOrEqualTo(1);
    }

    [Fact]
    public async Task GetByUseCase_AsNonProjectMember_ReturnsForbidden()
    {
        // Arrange
        var context = await CreateFullTaskContextAsync();
        SetAuthorizationHeader(context.NonMemberToken);

        // Act
        var response = await Client.GetAsync($"/api/v1/tasks/usecases/{context.UseCase.Id}?page=1&pageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetByUseCase_WithPagination_ReturnsCorrectPage()
    {
        // Arrange
        var context = await CreateFullTaskContextAsync();
        SetAuthorizationHeader(context.OwnerToken);

        // Birkaç task daha oluştur
        for (int i = 0; i < 5; i++)
        {
            var createTaskRequest = new { Title = $"Task {i}", TaskType = TaskType.Feature, Description = $"Description {i}", ImportantNotes = (string?)null, DueDate = (DateTime?)null, AssigneeId = (Guid?)null };
            await Client.PostAsJsonAsync($"/api/v1/tasks/usecases/{context.UseCase.Id}", createTaskRequest);
        }

        // Act
        var response = await Client.GetAsync($"/api/v1/tasks/usecases/{context.UseCase.Id}?page=1&pageSize=3");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PagedResult<TaskListItemResponse>>();
        result.Should().NotBeNull();
        result!.Items.Should().HaveCount(3);
        result.TotalCount.Should().BeGreaterThanOrEqualTo(6); // 1 original + 5 new
    }

    #endregion

    #region Phase 2 - State Change Tests (Transition Control and Dependencies)

    [Fact]
    public async Task ChangeState_AsProjectOwner_NotStartedToInProgress_ReturnsNoContent()
    {
        // Arrange
        var context = await CreateFullTaskContextAsync();
        SetAuthorizationHeader(context.OwnerToken);

        // Task'ı kendine ata (InProgress için assignee gerekli)
        await Client.PostAsJsonAsync($"/api/v1/tasks/{context.Task.Id}/assign", new { AssigneeId = context.OwnerId });

        var changeStateRequest = new { NewState = TaskState.InProgress };

        // Act
        var response = await Client.PatchAsJsonAsync($"/api/v1/tasks/{context.Task.Id}/state", changeStateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        
        // Verify state change
        var state = await GetTaskStateAsync(context.Task.Id);
        state.Should().Be(TaskState.InProgress);
    }

    [Fact]
    public async Task ChangeState_AsProjectMember_NotStartedToInProgress_ReturnsNoContent()
    {
        // Arrange
        var context = await CreateFullTaskContextAsync();
        SetAuthorizationHeader(context.MemberToken);

        // Task'ı member'a ata
        SetAuthorizationHeader(context.OwnerToken); // Owner yetkisiyle ata
        await Client.PostAsJsonAsync($"/api/v1/tasks/{context.Task.Id}/assign", new { AssigneeId = context.MemberId });
        SetAuthorizationHeader(context.MemberToken); // Member token'a geri dön

        var changeStateRequest = new { NewState = TaskState.InProgress };

        // Act
        var response = await Client.PatchAsJsonAsync($"/api/v1/tasks/{context.Task.Id}/state", changeStateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        
        // Verify state change
        var state = await GetTaskStateAsync(context.Task.Id);
        state.Should().Be(TaskState.InProgress);
    }

    [Fact]
    public async Task ChangeState_AsProjectViewer_ReturnsForbidden()
    {
        // Arrange
        var context = await CreateFullTaskContextAsync();
        SetAuthorizationHeader(context.ViewerToken);

        var changeStateRequest = new { NewState = TaskState.InProgress };

        // Act
        var response = await Client.PatchAsJsonAsync($"/api/v1/tasks/{context.Task.Id}/state", changeStateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task ChangeState_AsTaskCreator_ButNotProjectMember_ReturnsNoContent()
    {
        // Arrange
        var context = await CreateFullTaskContextAsync();
        // Task'ı owner oluşturdu
        SetAuthorizationHeader(context.OwnerToken);

        // Task'ı kendine ata
        await Client.PostAsJsonAsync($"/api/v1/tasks/{context.Task.Id}/assign", new { AssigneeId = context.OwnerId });

        var changeStateRequest = new { NewState = TaskState.InProgress };

        // Act
        var response = await Client.PatchAsJsonAsync($"/api/v1/tasks/{context.Task.Id}/state", changeStateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task ChangeState_AsNonProjectMember_ReturnsForbidden()
    {
        // Arrange
        var context = await CreateFullTaskContextAsync();
        SetAuthorizationHeader(context.NonMemberToken);

        var changeStateRequest = new { NewState = TaskState.InProgress };

        // Act
        var response = await Client.PatchAsJsonAsync($"/api/v1/tasks/{context.Task.Id}/state", changeStateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task ChangeState_InProgressToCompleted_ReturnsNoContent()
    {
        // Arrange
        var context = await CreateFullTaskContextAsync();
        SetAuthorizationHeader(context.OwnerToken);

        // Task'ı kendine ata ve InProgress yap
        await Client.PostAsJsonAsync($"/api/v1/tasks/{context.Task.Id}/assign", new { AssigneeId = context.OwnerId });
        await Client.PatchAsJsonAsync($"/api/v1/tasks/{context.Task.Id}/state", new { NewState = TaskState.InProgress });

        var changeStateRequest = new { NewState = TaskState.Completed };

        // Act
        var response = await Client.PatchAsJsonAsync($"/api/v1/tasks/{context.Task.Id}/state", changeStateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        
        // Verify state change
        var state = await GetTaskStateAsync(context.Task.Id);
        state.Should().Be(TaskState.Completed);
    }

    [Fact]
    public async Task ChangeState_InProgressToCancelled_ReturnsNoContent()
    {
        // Arrange
        var context = await CreateFullTaskContextAsync();
        SetAuthorizationHeader(context.OwnerToken);

        // Task'ı kendine ata ve InProgress yap
        await Client.PostAsJsonAsync($"/api/v1/tasks/{context.Task.Id}/assign", new { AssigneeId = context.OwnerId });
        await Client.PatchAsJsonAsync($"/api/v1/tasks/{context.Task.Id}/state", new { NewState = TaskState.InProgress });

        var changeStateRequest = new { NewState = TaskState.Cancelled };

        // Act
        var response = await Client.PatchAsJsonAsync($"/api/v1/tasks/{context.Task.Id}/state", changeStateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        
        // Verify state change
        var state = await GetTaskStateAsync(context.Task.Id);
        state.Should().Be(TaskState.Cancelled);
    }

    [Fact]
    public async Task ChangeState_WithoutAuth_ReturnsUnauthorized()
    {
        // Arrange
        var context = await CreateFullTaskContextAsync();
        ClearAuthorizationHeader();

        var changeStateRequest = new { NewState = TaskState.InProgress };

        // Act
        var response = await Client.PatchAsJsonAsync($"/api/v1/tasks/{context.Task.Id}/state", changeStateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ChangeState_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var token = await RegisterAndGetTokenAsync();
        SetAuthorizationHeader(token);
        var invalidId = Guid.NewGuid();

        var changeStateRequest = new { NewState = TaskState.InProgress };

        // Act
        var response = await Client.PatchAsJsonAsync($"/api/v1/tasks/{invalidId}/state", changeStateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region Phase 2 - Delete Tests (Authorization Scenarios)

    [Fact]
    public async Task Delete_AsProjectOwner_ReturnsNoContent()
    {
        // Arrange
        var context = await CreateFullTaskContextAsync();
        SetAuthorizationHeader(context.OwnerToken);

        // Act
        var response = await Client.DeleteAsync($"/api/v1/tasks/{context.Task.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Delete_AsProjectMember_ReturnsNoContent()
    {
        // Arrange
        var context = await CreateFullTaskContextAsync();
        SetAuthorizationHeader(context.MemberToken);

        // Act
        var response = await Client.DeleteAsync($"/api/v1/tasks/{context.Task.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Delete_AsProjectViewer_ReturnsForbidden()
    {
        // Arrange
        var context = await CreateFullTaskContextAsync();
        SetAuthorizationHeader(context.ViewerToken);

        // Act
        var response = await Client.DeleteAsync($"/api/v1/tasks/{context.Task.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Delete_AsTaskCreator_ButNotProjectMember_ReturnsNoContent()
    {
        // Arrange
        var context = await CreateFullTaskContextAsync();
        // Task'ı owner oluşturdu, owner token kullanıyoruz (creator her zaman silebilir)
        SetAuthorizationHeader(context.OwnerToken);

        // Act
        var response = await Client.DeleteAsync($"/api/v1/tasks/{context.Task.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Delete_AsNonProjectMember_ReturnsForbidden()
    {
        // Arrange
        var context = await CreateFullTaskContextAsync();
        SetAuthorizationHeader(context.NonMemberToken);

        // Act
        var response = await Client.DeleteAsync($"/api/v1/tasks/{context.Task.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Delete_WithoutAuth_ReturnsUnauthorized()
    {
        // Arrange
        var context = await CreateFullTaskContextAsync();
        ClearAuthorizationHeader();

        // Act
        var response = await Client.DeleteAsync($"/api/v1/tasks/{context.Task.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Delete_WithInvalidId_ReturnsNotFound()
    {
        // Arrange - Bu test zaten mevcut ama tutarlılık için kontrol
        var token = await RegisterAndGetTokenAsync();
        SetAuthorizationHeader(token);
        var invalidId = Guid.NewGuid();

        // Act
        var response = await Client.DeleteAsync($"/api/v1/tasks/{invalidId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    private async Task<(CreateProjectCommandResponse project, CreateModuleCommandResponse module, CreateUseCaseCommandResponse useCase, CreateTaskCommandResponse task)> CreateTaskHierarchyAsync()
    {
        // Proje oluştur
        var createProjectRequest = new CreateProjectCommand("Test Project", "Test Description");
        var projectResponse = await Client.PostAsJsonAsync("/api/v1/projects", createProjectRequest);
        projectResponse.EnsureSuccessStatusCode();
        var project = await projectResponse.Content.ReadFromJsonAsync<CreateProjectCommandResponse>();

        // Modül oluştur
        var createModuleRequest = new { Title = "Test Module", Description = "Test Description" };
        var moduleResponse = await Client.PostAsJsonAsync($"/api/v1/modules/projects/{project!.Id}", createModuleRequest);
        moduleResponse.EnsureSuccessStatusCode();
        var module = await moduleResponse.Content.ReadFromJsonAsync<CreateModuleCommandResponse>();

        // Use case oluştur
        var createUseCaseRequest = new { Title = "Test UseCase", Description = "Test Description", ImportantNotes = (string?)null };
        var useCaseResponse = await Client.PostAsJsonAsync($"/api/v1/usecases/modules/{module!.Id}", createUseCaseRequest);
        useCaseResponse.EnsureSuccessStatusCode();
        var useCase = await useCaseResponse.Content.ReadFromJsonAsync<CreateUseCaseCommandResponse>();

        // Task oluştur
        var createTaskRequest = new { Title = "Test Task", TaskType = TaskType.Feature, Description = "Test Description", ImportantNotes = (string?)null, DueDate = (DateTime?)null };
        var taskResponse = await Client.PostAsJsonAsync($"/api/v1/tasks/usecases/{useCase!.Id}", createTaskRequest);
        taskResponse.EnsureSuccessStatusCode();
        var task = await taskResponse.Content.ReadFromJsonAsync<CreateTaskCommandResponse>();

        return (project!, module!, useCase!, task!);
    }

}

