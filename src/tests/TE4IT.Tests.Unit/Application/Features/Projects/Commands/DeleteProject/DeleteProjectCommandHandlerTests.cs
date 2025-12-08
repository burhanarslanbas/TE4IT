using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using TE4IT.Abstractions.Persistence.Repositories.Projects;
using TE4IT.Application.Abstractions.Auth;
using TE4IT.Application.Abstractions.Persistence;
using TE4IT.Application.Features.Projects.Commands.DeleteProject;
using TE4IT.Domain.Exceptions.Projects;
using TE4IT.Domain.Services;
using Project = TE4IT.Domain.Entities.Project;
using TE4IT.Domain.ValueObjects;
using TE4IT.Tests.Unit.Common.Builders;
using Xunit;

namespace TE4IT.Tests.Unit.Application.Features.Projects.Commands.DeleteProject;

public class DeleteProjectCommandHandlerTests
{
    private readonly Mock<IProjectReadRepository> _readRepositoryMock;
    private readonly Mock<IProjectWriteRepository> _writeRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ICurrentUser> _currentUserMock;
    private readonly Mock<IUserPermissionService> _userPermissionServiceMock;

    public DeleteProjectCommandHandlerTests()
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
        var project = ProjectBuilder.Create().Build();
        project.Id = projectId;

        var command = new DeleteProjectCommand(projectId);
        _currentUserMock.Setup(x => x.Id).Returns((UserId)userId);
        _readRepositoryMock.Setup(x => x.GetByIdAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);
        _userPermissionServiceMock.Setup(x => x.CanDeleteProject(It.IsAny<UserId>(), project))
            .Returns(true);
        _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var handler = new DeleteProjectCommandHandler(
            _readRepositoryMock.Object,
            _writeRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _currentUserMock.Object,
            _userPermissionServiceMock.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WhenProjectNotFound_ReturnsFalse()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new DeleteProjectCommand(Guid.NewGuid());
        _currentUserMock.Setup(x => x.Id).Returns((UserId)userId);
        _readRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Project?)null);

        var handler = new DeleteProjectCommandHandler(
            _readRepositoryMock.Object,
            _writeRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _currentUserMock.Object,
            _userPermissionServiceMock.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeFalse();
        _writeRepositoryMock.Verify(x => x.Remove(It.IsAny<Project>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenUserCannotDeleteProject_ThrowsProjectAccessDeniedException()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var project = ProjectBuilder.Create().Build();
        project.Id = projectId;

        var command = new DeleteProjectCommand(projectId);
        _currentUserMock.Setup(x => x.Id).Returns((UserId)userId);
        _readRepositoryMock.Setup(x => x.GetByIdAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);
        _userPermissionServiceMock.Setup(x => x.CanDeleteProject(It.IsAny<UserId>(), project))
            .Returns(false); // Kullanıcının silme yetkisi yok

        var handler = new DeleteProjectCommandHandler(
            _readRepositoryMock.Object,
            _writeRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _currentUserMock.Object,
            _userPermissionServiceMock.Object);

        // Act
        var act = async () => await handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ProjectAccessDeniedException>();
        _writeRepositoryMock.Verify(x => x.Remove(It.IsAny<Project>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_CallsRepositoryRemove()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var project = ProjectBuilder.Create().Build();
        project.Id = projectId;

        var command = new DeleteProjectCommand(projectId);
        _currentUserMock.Setup(x => x.Id).Returns((UserId)userId);
        _readRepositoryMock.Setup(x => x.GetByIdAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);
        _userPermissionServiceMock.Setup(x => x.CanDeleteProject(It.IsAny<UserId>(), project))
            .Returns(true);
        _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var handler = new DeleteProjectCommandHandler(
            _readRepositoryMock.Object,
            _writeRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _currentUserMock.Object,
            _userPermissionServiceMock.Object);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        _writeRepositoryMock.Verify(x => x.Remove(It.Is<Project>(p => p.Id == projectId), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_CallsUnitOfWorkSaveChangesAsync()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var project = ProjectBuilder.Create().Build();
        project.Id = projectId;

        var command = new DeleteProjectCommand(projectId);
        _currentUserMock.Setup(x => x.Id).Returns((UserId)userId);
        _readRepositoryMock.Setup(x => x.GetByIdAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);
        _userPermissionServiceMock.Setup(x => x.CanDeleteProject(It.IsAny<UserId>(), project))
            .Returns(true);
        _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var handler = new DeleteProjectCommandHandler(
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

