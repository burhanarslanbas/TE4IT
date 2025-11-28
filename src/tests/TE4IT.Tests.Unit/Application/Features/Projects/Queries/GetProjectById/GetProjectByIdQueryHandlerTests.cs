using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using TE4IT.Abstractions.Persistence.Repositories.Projects;
using TE4IT.Application.Features.Projects.Queries.GetProjectById;
using Project = TE4IT.Domain.Entities.Project;
using TE4IT.Tests.Unit.Common.Builders;
using Xunit;

namespace TE4IT.Tests.Unit.Application.Features.Projects.Queries.GetProjectById;

public class GetProjectByIdQueryHandlerTests
{
    private readonly Mock<IProjectReadRepository> _repositoryMock;
    private readonly GetProjectByIdQueryHandler _handler;

    public GetProjectByIdQueryHandlerTests()
    {
        _repositoryMock = new Mock<IProjectReadRepository>();
        _handler = new GetProjectByIdQueryHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidId_ReturnsProjectResponse()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var project = ProjectBuilder.Create()
            .WithTitle("Test Project")
            .WithDescription("Test Description")
            .WithIsActive(true)
            .Build();
        project.Id = projectId;

        var query = new GetProjectByIdQuery(projectId);
        _repositoryMock.Setup(x => x.GetByIdAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(projectId);
        result.Title.Should().Be("Test Project");
        result.Description.Should().Be("Test Description");
        result.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WhenProjectNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        var query = new GetProjectByIdQuery(Guid.NewGuid());
        _repositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Project?)null);

        // Act
        var act = async () => await _handler.Handle(query, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("Project not found");
    }

    [Fact]
    public async Task Handle_MapsProjectToResponseCorrectly()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var creatorId = Guid.NewGuid();
        var project = ProjectBuilder.Create()
            .WithCreatorId(creatorId)
            .WithTitle("Mapped Project")
            .WithDescription("Mapped Description")
            .WithIsActive(false)
            .Build();
        project.Id = projectId;
        // Note: StartedDate is set in Create method, we can't easily override it in tests
        // This test verifies mapping works correctly with the actual StartedDate

        var query = new GetProjectByIdQuery(projectId);
        _repositoryMock.Setup(x => x.GetByIdAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Id.Should().Be(projectId);
        result.Title.Should().Be("Mapped Project");
        result.Description.Should().Be("Mapped Description");
        result.IsActive.Should().BeFalse();
        result.StartedDate.Should().BeCloseTo(project.StartedDate, TimeSpan.FromSeconds(1));
    }
}

