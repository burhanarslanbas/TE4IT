using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TE4IT.Abstractions.Persistence.Repositories.Projects;
using Project = TE4IT.Domain.Entities.Project;
using TE4IT.Domain.ValueObjects;
using TE4IT.Persistence.TaskManagement.Repositories.Projects;
using TE4IT.Tests.Integration.Common;
using Xunit;

namespace TE4IT.Tests.Integration.Persistence.Repositories.Projects;

public class ProjectWriteRepositoryTests : IntegrationTestBase
{
    private readonly IProjectWriteRepository _repository;

    public ProjectWriteRepositoryTests()
    {
        _repository = new ProjectWriteRepository(DbContext);
    }

    [Fact]
    public async Task AddAsync_WithValidProject_AddsToDatabase()
    {
        // Arrange
        var creatorId = new UserId(Guid.NewGuid());
        var project = Project.Create(creatorId, "Test Project", "Test Description");

        // Act
        await _repository.AddAsync(project, CancellationToken.None);
        await DbContext.SaveChangesAsync(CancellationToken.None);

        // Assert
        var savedProject = await DbContext.Projects.FirstOrDefaultAsync(p => p.Id == project.Id);
        savedProject.Should().NotBeNull();
        savedProject!.Title.Should().Be("Test Project");
        savedProject.Description.Should().Be("Test Description");
        savedProject.CreatorId.Should().Be(creatorId);
    }

    [Fact]
    public async Task Update_WithValidProject_UpdatesInDatabase()
    {
        // Arrange
        var creatorId = new UserId(Guid.NewGuid());
        var project = Project.Create(creatorId, "Original Title", "Original Description");
        await SeedDataAsync(project);

        // Act
        project.UpdateTitle("Updated Title");
        project.UpdateDescription("Updated Description");
        _repository.Update(project, CancellationToken.None);
        await DbContext.SaveChangesAsync(CancellationToken.None);

        // Assert
        var updatedProject = await DbContext.Projects.FirstOrDefaultAsync(p => p.Id == project.Id);
        updatedProject.Should().NotBeNull();
        updatedProject!.Title.Should().Be("Updated Title");
        updatedProject.Description.Should().Be("Updated Description");
    }

    [Fact]
    public async Task Remove_WithValidProject_RemovesFromDatabase()
    {
        // Arrange
        var creatorId = new UserId(Guid.NewGuid());
        var project = Project.Create(creatorId, "Test Project", "Test Description");
        await SeedDataAsync(project);

        // Act
        _repository.Remove(project, CancellationToken.None);
        await DbContext.SaveChangesAsync(CancellationToken.None);

        // Assert
        var removedProject = await DbContext.Projects.FirstOrDefaultAsync(p => p.Id == project.Id);
        removedProject.Should().BeNull();
    }

    [Fact]
    public async Task AddAsync_SetsCreatedDate()
    {
        // Arrange
        var creatorId = new UserId(Guid.NewGuid());
        var project = Project.Create(creatorId, "Test Project", null);

        // Act
        await _repository.AddAsync(project, CancellationToken.None);
        await DbContext.SaveChangesAsync(CancellationToken.None);

        // Assert
        var savedProject = await DbContext.Projects.FirstOrDefaultAsync(p => p.Id == project.Id);
        savedProject.Should().NotBeNull();
        savedProject!.CreatedDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public async Task Update_SetsUpdatedDate()
    {
        // Arrange
        var creatorId = new UserId(Guid.NewGuid());
        var project = Project.Create(creatorId, "Original Title", null);
        await SeedDataAsync(project);
        var originalUpdatedDate = project.UpdatedDate ?? DateTime.UtcNow;

        // Act
        await Task.Delay(100); // Small delay to ensure different timestamp
        project.UpdateTitle("Updated Title");
        _repository.Update(project, CancellationToken.None);
        await DbContext.SaveChangesAsync(CancellationToken.None);

        // Assert
        var updatedProject = await DbContext.Projects.FirstOrDefaultAsync(p => p.Id == project.Id);
        updatedProject.Should().NotBeNull();
        updatedProject!.UpdatedDate.Should().NotBeNull();
        updatedProject.UpdatedDate!.Value.Should().BeAfter(originalUpdatedDate);
    }
}

