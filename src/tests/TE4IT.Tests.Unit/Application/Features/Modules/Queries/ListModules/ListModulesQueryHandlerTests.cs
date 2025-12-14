using FluentAssertions;
using Moq;
using TE4IT.Abstractions.Persistence.Repositories.Modules;
using TE4IT.Abstractions.Persistence.Repositories.Projects;
using TE4IT.Abstractions.Persistence.Repositories.UseCases;
using TE4IT.Application.Abstractions.Auth;
using TE4IT.Application.Common.Pagination;
using TE4IT.Application.Features.Modules.Queries.ListModules;
using TE4IT.Domain.Exceptions.Common;
using TE4IT.Domain.Exceptions.Projects;
using TE4IT.Domain.Services;
using TE4IT.Domain.ValueObjects;
using TE4IT.Tests.Unit.Common.Builders;
using Xunit;
using Module = TE4IT.Domain.Entities.Module;
using Project = TE4IT.Domain.Entities.Project;

namespace TE4IT.Tests.Unit.Application.Features.Modules.Queries.ListModules;

public class ListModulesQueryHandlerTests
{
    private readonly Mock<IModuleReadRepository> _moduleRepositoryMock;
    private readonly Mock<IUseCaseReadRepository> _useCaseRepositoryMock;
    private readonly Mock<IProjectReadRepository> _projectRepositoryMock;
    private readonly Mock<ICurrentUser> _currentUserMock;
    private readonly Mock<IUserPermissionService> _userPermissionServiceMock;

    public ListModulesQueryHandlerTests()
    {
        _moduleRepositoryMock = new Mock<IModuleReadRepository>();
        _useCaseRepositoryMock = new Mock<IUseCaseReadRepository>();
        _projectRepositoryMock = new Mock<IProjectReadRepository>();
        _currentUserMock = new Mock<ICurrentUser>();
        _userPermissionServiceMock = new Mock<IUserPermissionService>();
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_WithValidQuery_ReturnsPagedResult()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        
        var project = ProjectBuilder.Create().Build();
        project.Id = projectId;
        
        var modules = new List<Module>
        {
            ModuleBuilder.Create().WithProjectId(projectId).Build(),
            ModuleBuilder.Create().WithProjectId(projectId).Build(),
            ModuleBuilder.Create().WithProjectId(projectId).Build()
        };
        modules[0].Id = Guid.NewGuid();
        modules[1].Id = Guid.NewGuid();
        modules[2].Id = Guid.NewGuid();

        var moduleIds = modules.Select(m => m.Id).ToList();
        var useCaseCounts = new Dictionary<Guid, int>
        {
            { modules[0].Id, 5 },
            { modules[1].Id, 3 },
            { modules[2].Id, 0 }
        };

        var pagedModules = new PagedResult<Module>(modules, 3, 1, 10);
        var query = new ListModulesQuery(projectId, 1, 10, null, null);

        _currentUserMock.Setup(x => x.Id).Returns((UserId)userId);
        _projectRepositoryMock.Setup(x => x.GetByIdAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);
        _userPermissionServiceMock.Setup(x => x.CanAccessProject(It.IsAny<UserId>(), project))
            .Returns(true);
        _moduleRepositoryMock.Setup(x => x.GetByProjectIdAsync(
                projectId, 1, 10, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedModules);
        _useCaseRepositoryMock.Setup(x => x.CountByModuleIdsAsync(
                It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(useCaseCounts);

        var handler = new ListModulesQueryHandler(
            _moduleRepositoryMock.Object,
            _useCaseRepositoryMock.Object,
            _projectRepositoryMock.Object,
            _currentUserMock.Object,
            _userPermissionServiceMock.Object);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(3);
        result.TotalCount.Should().Be(3);
        result.Items[0].UseCaseCount.Should().Be(5);
        result.Items[1].UseCaseCount.Should().Be(3);
        result.Items[2].UseCaseCount.Should().Be(0);
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_CallsCountByModuleIdsAsyncOnce_AvoidingN1Query()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        
        var project = ProjectBuilder.Create().Build();
        project.Id = projectId;
        
        var modules = new List<Module>
        {
            ModuleBuilder.Create().WithProjectId(projectId).Build(),
            ModuleBuilder.Create().WithProjectId(projectId).Build(),
            ModuleBuilder.Create().WithProjectId(projectId).Build()
        };
        modules[0].Id = Guid.NewGuid();
        modules[1].Id = Guid.NewGuid();
        modules[2].Id = Guid.NewGuid();

        var moduleIds = modules.Select(m => m.Id).ToList();
        var useCaseCounts = new Dictionary<Guid, int>
        {
            { modules[0].Id, 2 },
            { modules[1].Id, 4 },
            { modules[2].Id, 1 }
        };

        var pagedModules = new PagedResult<Module>(modules, 3, 1, 10);
        var query = new ListModulesQuery(projectId, 1, 10, null, null);

        _currentUserMock.Setup(x => x.Id).Returns((UserId)userId);
        _projectRepositoryMock.Setup(x => x.GetByIdAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);
        _userPermissionServiceMock.Setup(x => x.CanAccessProject(It.IsAny<UserId>(), project))
            .Returns(true);
        _moduleRepositoryMock.Setup(x => x.GetByProjectIdAsync(
                projectId, 1, 10, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedModules);
        _useCaseRepositoryMock.Setup(x => x.CountByModuleIdsAsync(
                It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(useCaseCounts);

        var handler = new ListModulesQueryHandler(
            _moduleRepositoryMock.Object,
            _useCaseRepositoryMock.Object,
            _projectRepositoryMock.Object,
            _currentUserMock.Object,
            _userPermissionServiceMock.Object);

        // Act
        await handler.Handle(query, CancellationToken.None);

        // Assert - CountByModuleIdsAsync sadece bir kez çağrılmalı (N+1 query yok)
        _useCaseRepositoryMock.Verify(
            x => x.CountByModuleIdsAsync(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()), 
            Times.Once);
        
        // CountByModuleAsync hiç çağrılmamalı (eski N+1 yöntem)
        _useCaseRepositoryMock.Verify(
            x => x.CountByModuleAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), 
            Times.Never);
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_WhenProjectNotFound_ThrowsResourceNotFoundException()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var query = new ListModulesQuery(projectId, 1, 10, null, null);

        _currentUserMock.Setup(x => x.Id).Returns((UserId)userId);
        _projectRepositoryMock.Setup(x => x.GetByIdAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Project?)null);

        var handler = new ListModulesQueryHandler(
            _moduleRepositoryMock.Object,
            _useCaseRepositoryMock.Object,
            _projectRepositoryMock.Object,
            _currentUserMock.Object,
            _userPermissionServiceMock.Object);

        // Act
        var act = async () => await handler.Handle(query, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ResourceNotFoundException>();
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_WhenUserCannotAccessProject_ThrowsProjectAccessDeniedException()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        
        var project = ProjectBuilder.Create().Build();
        project.Id = projectId;
        
        var query = new ListModulesQuery(projectId, 1, 10, null, null);

        _currentUserMock.Setup(x => x.Id).Returns((UserId)userId);
        _projectRepositoryMock.Setup(x => x.GetByIdAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);
        _userPermissionServiceMock.Setup(x => x.CanAccessProject(It.IsAny<UserId>(), project))
            .Returns(false);

        var handler = new ListModulesQueryHandler(
            _moduleRepositoryMock.Object,
            _useCaseRepositoryMock.Object,
            _projectRepositoryMock.Object,
            _currentUserMock.Object,
            _userPermissionServiceMock.Object);

        // Act
        var act = async () => await handler.Handle(query, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ProjectAccessDeniedException>();
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_WhenModuleNotInDictionary_UsesZeroCount()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        
        var project = ProjectBuilder.Create().Build();
        project.Id = projectId;
        
        var modules = new List<Module>
        {
            ModuleBuilder.Create().WithProjectId(projectId).Build(),
            ModuleBuilder.Create().WithProjectId(projectId).Build()
        };
        modules[0].Id = Guid.NewGuid();
        modules[1].Id = Guid.NewGuid();

        // Sadece birinci modül için count var, ikincisi için yok
        var useCaseCounts = new Dictionary<Guid, int>
        {
            { modules[0].Id, 3 }
        };

        var pagedModules = new PagedResult<Module>(modules, 2, 1, 10);
        var query = new ListModulesQuery(projectId, 1, 10, null, null);

        _currentUserMock.Setup(x => x.Id).Returns((UserId)userId);
        _projectRepositoryMock.Setup(x => x.GetByIdAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);
        _userPermissionServiceMock.Setup(x => x.CanAccessProject(It.IsAny<UserId>(), project))
            .Returns(true);
        _moduleRepositoryMock.Setup(x => x.GetByProjectIdAsync(
                projectId, 1, 10, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedModules);
        _useCaseRepositoryMock.Setup(x => x.CountByModuleIdsAsync(
                It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(useCaseCounts);

        var handler = new ListModulesQueryHandler(
            _moduleRepositoryMock.Object,
            _useCaseRepositoryMock.Object,
            _projectRepositoryMock.Object,
            _currentUserMock.Object,
            _userPermissionServiceMock.Object);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Items[0].UseCaseCount.Should().Be(3);
        result.Items[1].UseCaseCount.Should().Be(0); // Dictionary'de yoksa 0 olmalı
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_WithEmptyModuleList_ReturnsEmptyResult()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        
        var project = ProjectBuilder.Create().Build();
        project.Id = projectId;
        
        var pagedModules = new PagedResult<Module>(new List<Module>(), 0, 1, 10);
        var query = new ListModulesQuery(projectId, 1, 10, null, null);

        _currentUserMock.Setup(x => x.Id).Returns((UserId)userId);
        _projectRepositoryMock.Setup(x => x.GetByIdAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);
        _userPermissionServiceMock.Setup(x => x.CanAccessProject(It.IsAny<UserId>(), project))
            .Returns(true);
        _moduleRepositoryMock.Setup(x => x.GetByProjectIdAsync(
                projectId, 1, 10, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedModules);
        _useCaseRepositoryMock.Setup(x => x.CountByModuleIdsAsync(
                It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<Guid, int>());

        var handler = new ListModulesQueryHandler(
            _moduleRepositoryMock.Object,
            _useCaseRepositoryMock.Object,
            _projectRepositoryMock.Object,
            _currentUserMock.Object,
            _userPermissionServiceMock.Object);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
    }
}

