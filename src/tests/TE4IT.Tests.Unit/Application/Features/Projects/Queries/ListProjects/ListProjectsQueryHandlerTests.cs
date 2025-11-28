using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using TE4IT.Abstractions.Persistence.Repositories.Projects;
using TE4IT.Application.Common.Pagination;
using TE4IT.Application.Features.Projects.Queries.ListProjects;
using Project = TE4IT.Domain.Entities.Project;
using TE4IT.Tests.Unit.Common.Builders;
using Xunit;

namespace TE4IT.Tests.Unit.Application.Features.Projects.Queries.ListProjects;

public class ListProjectsQueryHandlerTests
{
    private readonly Mock<IProjectReadRepository> _repositoryMock;
    private readonly ListProjectsQueryHandler _handler;

    public ListProjectsQueryHandlerTests()
    {
        _repositoryMock = new Mock<IProjectReadRepository>();
        _handler = new ListProjectsQueryHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidQuery_ReturnsPagedResult()
    {
        // Arrange
        var projects = new List<Project>
        {
            ProjectBuilder.Create().WithTitle("Project 1").Build(),
            ProjectBuilder.Create().WithTitle("Project 2").Build()
        };

        var pagedResult = new PagedResult<Project>(projects, 2, 1, 20);
        var query = new ListProjectsQuery(1, 20);
        _repositoryMock.Setup(x => x.ListAsync(1, 20, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResult);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(2);
        result.TotalCount.Should().Be(2);
        result.Page.Should().Be(1);
        result.PageSize.Should().Be(20);
    }

    [Fact]
    public async Task Handle_MapsProjectsToResponsesCorrectly()
    {
        // Arrange
        var project1 = ProjectBuilder.Create()
            .WithTitle("Project 1")
            .WithIsActive(true)
            .Build();
        var project2 = ProjectBuilder.Create()
            .WithTitle("Project 2")
            .WithIsActive(false)
            .Build();

        var projects = new List<Project> { project1, project2 };
        var pagedResult = new PagedResult<Project>(projects, 2, 1, 20);
        var query = new ListProjectsQuery(1, 20);
        _repositoryMock.Setup(x => x.ListAsync(1, 20, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResult);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Items.Should().HaveCount(2);
        result.Items[0].Title.Should().Be("Project 1");
        result.Items[0].IsActive.Should().BeTrue();
        result.Items[1].Title.Should().Be("Project 2");
        result.Items[1].IsActive.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_WithPagination_ReturnsCorrectPage()
    {
        // Arrange
        var projects = new List<Project>
        {
            ProjectBuilder.Create().WithTitle("Project 1").Build()
        };

        var pagedResult = new PagedResult<Project>(projects, 25, 2, 10);
        var query = new ListProjectsQuery(2, 10);
        _repositoryMock.Setup(x => x.ListAsync(2, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResult);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Page.Should().Be(2);
        result.PageSize.Should().Be(10);
        result.TotalCount.Should().Be(25);
        result.Items.Should().HaveCount(1);
    }

    [Fact]
    public async Task Handle_WithEmptyList_ReturnsEmptyPagedResult()
    {
        // Arrange
        var pagedResult = new PagedResult<Project>(new List<Project>(), 0, 1, 20);
        var query = new ListProjectsQuery(1, 20);
        _repositoryMock.Setup(x => x.ListAsync(1, 20, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResult);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Items.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
    }
}

