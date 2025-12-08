using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using TE4IT.Abstractions.Persistence.Repositories.Projects;
using TE4IT.Application.Abstractions.Auth;
using TE4IT.Application.Abstractions.Persistence;
using TE4IT.Application.Features.Projects.Commands.UpdateProject;
using TE4IT.Domain.Exceptions.Projects;
using TE4IT.Domain.Services;
using Project = TE4IT.Domain.Entities.Project;
using TE4IT.Domain.ValueObjects;
using TE4IT.Tests.Unit.Common.Builders;
using Xunit;

namespace TE4IT.Tests.Unit.Application.Features.Projects.Commands.UpdateProject;

public class UpdateProjectCommandHandlerTests
{
    private readonly Mock<IProjectReadRepository> _readRepositoryMock;
    private readonly Mock<IProjectWriteRepository> _writeRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ICurrentUser> _currentUserMock;
    private readonly Mock<IUserPermissionService> _userPermissionServiceMock;

    public UpdateProjectCommandHandlerTests()
    {
        _readRepositoryMock = new Mock<IProjectReadRepository>();
        _writeRepositoryMock = new Mock<IProjectWriteRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _currentUserMock = new Mock<ICurrentUser>();
        _userPermissionServiceMock = new Mock<IUserPermissionService>();
    }

    [Fact]
    public async Task Handle_WithValidCommand_ReturnsTrue()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var project = ProjectBuilder.Create()
            .WithTitle("Old Title")
            .WithDescription("Old Description")
            .Build();
        project.Id = projectId;

        var command = new UpdateProjectCommand(projectId, "New Title", "New Description");
        _currentUserMock.Setup(x => x.Id).Returns((UserId)userId);
        _readRepositoryMock.Setup(x => x.GetByIdAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);
        _userPermissionServiceMock.Setup(x => x.CanEditProject(It.IsAny<UserId>(), project))
            .Returns(true);
        _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var handler = new UpdateProjectCommandHandler(
            _readRepositoryMock.Object,
            _writeRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _currentUserMock.Object,
            _userPermissionServiceMock.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
        project.Title.Should().Be("New Title");
        project.Description.Should().Be("New Description");
    }

    [Fact]
    public async Task Handle_WhenProjectNotFound_ReturnsFalse()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new UpdateProjectCommand(Guid.NewGuid(), "New Title", "New Description");
        _currentUserMock.Setup(x => x.Id).Returns((UserId)userId);
        _readRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Project?)null);

        var handler = new UpdateProjectCommandHandler(
            _readRepositoryMock.Object,
            _writeRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _currentUserMock.Object,
            _userPermissionServiceMock.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeFalse();
        _writeRepositoryMock.Verify(x => x.Update(It.IsAny<Project>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenUserCannotEditProject_ThrowsProjectAccessDeniedException()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var project = ProjectBuilder.Create()
            .WithTitle("Old Title")
            .WithDescription("Old Description")
            .Build();
        project.Id = projectId;

        var command = new UpdateProjectCommand(projectId, "New Title", "New Description");
        _currentUserMock.Setup(x => x.Id).Returns((UserId)userId);
        _readRepositoryMock.Setup(x => x.GetByIdAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);
        _userPermissionServiceMock.Setup(x => x.CanEditProject(It.IsAny<UserId>(), project))
            .Returns(false); // Kullanıcının düzenleme yetkisi yok

        var handler = new UpdateProjectCommandHandler(
            _readRepositoryMock.Object,
            _writeRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _currentUserMock.Object,
            _userPermissionServiceMock.Object);

        // Act
        var act = async () => await handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ProjectAccessDeniedException>();
        _writeRepositoryMock.Verify(x => x.Update(It.IsAny<Project>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_CallsUpdateTitle()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var project = ProjectBuilder.Create()
            .WithTitle("Old Title")
            .Build();
        project.Id = projectId;

        var command = new UpdateProjectCommand(projectId, "New Title", null);
        _currentUserMock.Setup(x => x.Id).Returns((UserId)userId);
        _readRepositoryMock.Setup(x => x.GetByIdAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);
        _userPermissionServiceMock.Setup(x => x.CanEditProject(It.IsAny<UserId>(), project))
            .Returns(true);
        _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var handler = new UpdateProjectCommandHandler(
            _readRepositoryMock.Object,
            _writeRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _currentUserMock.Object,
            _userPermissionServiceMock.Object);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        project.Title.Should().Be("New Title");
    }

    [Fact]
    public async Task Handle_CallsUpdateDescription()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var project = ProjectBuilder.Create()
            .WithDescription("Old Description")
            .Build();
        project.Id = projectId;

        var command = new UpdateProjectCommand(projectId, "Title", "New Description");
        _currentUserMock.Setup(x => x.Id).Returns((UserId)userId);
        _readRepositoryMock.Setup(x => x.GetByIdAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);
        _userPermissionServiceMock.Setup(x => x.CanEditProject(It.IsAny<UserId>(), project))
            .Returns(true);
        _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var handler = new UpdateProjectCommandHandler(
            _readRepositoryMock.Object,
            _writeRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _currentUserMock.Object,
            _userPermissionServiceMock.Object);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        project.Description.Should().Be("New Description");
    }

    [Fact]
    public async Task Handle_CallsRepositoryUpdate()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var project = ProjectBuilder.Create().Build();
        project.Id = projectId;

        var command = new UpdateProjectCommand(projectId, "New Title", "New Description");
        _currentUserMock.Setup(x => x.Id).Returns((UserId)userId);
        _readRepositoryMock.Setup(x => x.GetByIdAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);
        _userPermissionServiceMock.Setup(x => x.CanEditProject(It.IsAny<UserId>(), project))
            .Returns(true);
        _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var handler = new UpdateProjectCommandHandler(
            _readRepositoryMock.Object,
            _writeRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _currentUserMock.Object,
            _userPermissionServiceMock.Object);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        _writeRepositoryMock.Verify(x => x.Update(It.Is<Project>(p => p.Id == projectId), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_CallsUnitOfWorkSaveChangesAsync()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var project = ProjectBuilder.Create().Build();
        project.Id = projectId;

        var command = new UpdateProjectCommand(projectId, "New Title", "New Description");
        _currentUserMock.Setup(x => x.Id).Returns((UserId)userId);
        _readRepositoryMock.Setup(x => x.GetByIdAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);
        _userPermissionServiceMock.Setup(x => x.CanEditProject(It.IsAny<UserId>(), project))
            .Returns(true);
        _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var handler = new UpdateProjectCommandHandler(
            _readRepositoryMock.Object,
            _writeRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _currentUserMock.Object,
            _userPermissionServiceMock.Object);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}

