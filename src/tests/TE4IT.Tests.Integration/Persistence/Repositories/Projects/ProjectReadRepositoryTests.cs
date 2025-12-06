using System.Threading.Tasks;
using FluentAssertions;
using TE4IT.Abstractions.Persistence.Repositories.Projects;
using Project = TE4IT.Domain.Entities.Project;
using TE4IT.Domain.ValueObjects;
using TE4IT.Persistence.TaskManagement.Repositories.Projects;
using TE4IT.Tests.Integration.Common;
using Xunit;

namespace TE4IT.Tests.Integration.Persistence.Repositories.Projects;

public class ProjectReadRepositoryTests : IntegrationTestBase
{
    private readonly IProjectReadRepository _repository;

    public ProjectReadRepositoryTests()
    {
        _repository = new ProjectReadRepository(DbContext);
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsProject()
    {
        // Arrange
        var creatorId = new UserId(Guid.NewGuid());
        var project = Project.Create(creatorId, "Test Project", "Test Description");
        await SeedDataAsync(project);

        // Act
        var result = await _repository.GetByIdAsync(project.Id, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(project.Id);
        result.Title.Should().Be("Test Project");
        result.Description.Should().Be("Test Description");
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ReturnsNull()
    {
        // Arrange
        var invalidId = Guid.NewGuid();

        // Act
        var result = await _repository.GetByIdAsync(invalidId, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task ListAsync_WithPagination_ReturnsPagedResult()
    {
        // Arrange
        var creatorId = new UserId(Guid.NewGuid());
        var projects = new List<Project>
        {
            Project.Create(creatorId, "Project 1", "Description 1"),
            Project.Create(creatorId, "Project 2", "Description 2"),
            Project.Create(creatorId, "Project 3", "Description 3")
        };
        await SeedDataAsync(projects.ToArray<object>());

        // Act
        var result = await _repository.ListAsync(1, 2, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(2);
        result.TotalCount.Should().Be(3);
        result.Page.Should().Be(1);
        result.PageSize.Should().Be(2);
    }

    [Fact]
    public async Task CountByCreatorAsync_WithValidCreatorId_ReturnsCount()
    {
        // Arrange
        var creatorId1 = new UserId(Guid.NewGuid());
        var creatorId2 = new UserId(Guid.NewGuid());
        
        var projects = new List<Project>
        {
            Project.Create(creatorId1, "Project 1", null),
            Project.Create(creatorId1, "Project 2", null),
            Project.Create(creatorId2, "Project 3", null)
        };
        await SeedDataAsync(projects.ToArray<object>());

        // Act
        var count = await _repository.CountByCreatorAsync(creatorId1.Value, CancellationToken.None);

        // Assert
        count.Should().Be(2);
    }

    [Fact]
    public async Task CountByCreatorAsync_WithInvalidCreatorId_ReturnsZero()
    {
        // Arrange
        var creatorId = new UserId(Guid.NewGuid());
        var project = Project.Create(creatorId, "Project 1", null);
        await SeedDataAsync(project);

        var invalidCreatorId = Guid.NewGuid();

        // Act
        var count = await _repository.CountByCreatorAsync(invalidCreatorId, CancellationToken.None);

        // Assert
        count.Should().Be(0);
    }

    [Fact]
    public async Task ListAsync_WithEmptyDatabase_ReturnsEmptyPagedResult()
    {
        // Act
        var result = await _repository.ListAsync(1, 20, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
    }
}

