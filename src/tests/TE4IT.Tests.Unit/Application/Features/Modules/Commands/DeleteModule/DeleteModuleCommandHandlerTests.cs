using FluentAssertions;
using Moq;
using TE4IT.Abstractions.Persistence.Repositories.Modules;
using TE4IT.Abstractions.Persistence.Repositories.Projects;
using TE4IT.Abstractions.Persistence.Repositories.UseCases;
using TE4IT.Application.Abstractions.Auth;
using TE4IT.Application.Abstractions.Persistence;
using TE4IT.Application.Features.Modules.Commands.DeleteModule;
using TE4IT.Domain.Exceptions.Common;
using TE4IT.Domain.Exceptions.Projects;
using TE4IT.Domain.Services;
using TE4IT.Domain.ValueObjects;
using TE4IT.Tests.Unit.Common.Builders;
using Xunit;
using Module = TE4IT.Domain.Entities.Module;
using Project = TE4IT.Domain.Entities.Project;

namespace TE4IT.Tests.Unit.Application.Features.Modules.Commands.DeleteModule;

public class DeleteModuleCommandHandlerTests
{
    private readonly Mock<IModuleReadRepository> _readRepositoryMock;
    private readonly Mock<IModuleWriteRepository> _writeRepositoryMock;
    private readonly Mock<IUseCaseReadRepository> _useCaseRepositoryMock;
    private readonly Mock<IProjectReadRepository> _projectReadRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ICurrentUser> _currentUserMock;
    private readonly Mock<IUserPermissionService> _userPermissionServiceMock;

    public DeleteModuleCommandHandlerTests()
    {
        _readRepositoryMock = new Mock<IModuleReadRepository>();
        _writeRepositoryMock = new Mock<IModuleWriteRepository>();
        _useCaseRepositoryMock = new Mock<IUseCaseReadRepository>();
        _projectReadRepositoryMock = new Mock<IProjectReadRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _currentUserMock = new Mock<ICurrentUser>();
        _userPermissionServiceMock = new Mock<IUserPermissionService>();
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_WithValidCommand_ReturnsTrue()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var moduleId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        
        var project = ProjectBuilder.Create().Build();
        project.Id = projectId;
        
        var module = ModuleBuilder.Create()
            .WithProjectId(projectId)
            .Build();
        module.Id = moduleId;

        var command = new DeleteModuleCommand(moduleId);
        _currentUserMock.Setup(x => x.Id).Returns((UserId)userId);
        _readRepositoryMock.Setup(x => x.GetByIdAsync(moduleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(module);
        _projectReadRepositoryMock.Setup(x => x.GetByIdAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);
        _useCaseRepositoryMock.Setup(x => x.CountByModuleAsync(moduleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(0); // UseCase yok
        _userPermissionServiceMock.Setup(x => x.CanEditProject(It.IsAny<UserId>(), project))
            .Returns(true);
        _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var handler = new DeleteModuleCommandHandler(
            _readRepositoryMock.Object,
            _writeRepositoryMock.Object,
            _useCaseRepositoryMock.Object,
            _projectReadRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _currentUserMock.Object,
            _userPermissionServiceMock.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
        _writeRepositoryMock.Verify(x => x.Remove(module, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_WhenModuleNotFound_ReturnsFalse()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new DeleteModuleCommand(Guid.NewGuid());
        _currentUserMock.Setup(x => x.Id).Returns((UserId)userId);
        _readRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Module?)null);

        var handler = new DeleteModuleCommandHandler(
            _readRepositoryMock.Object,
            _writeRepositoryMock.Object,
            _useCaseRepositoryMock.Object,
            _projectReadRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _currentUserMock.Object,
            _userPermissionServiceMock.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeFalse();
        _writeRepositoryMock.Verify(x => x.Remove(It.IsAny<Module>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_WhenUserCannotEditProject_ThrowsProjectAccessDeniedException()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var moduleId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        
        var project = ProjectBuilder.Create().Build();
        project.Id = projectId;
        
        var module = ModuleBuilder.Create().WithProjectId(projectId).Build();
        module.Id = moduleId;

        var command = new DeleteModuleCommand(moduleId);
        _currentUserMock.Setup(x => x.Id).Returns((UserId)userId);
        _readRepositoryMock.Setup(x => x.GetByIdAsync(moduleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(module);
        _projectReadRepositoryMock.Setup(x => x.GetByIdAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);
        _userPermissionServiceMock.Setup(x => x.CanEditProject(It.IsAny<UserId>(), project))
            .Returns(false);

        var handler = new DeleteModuleCommandHandler(
            _readRepositoryMock.Object,
            _writeRepositoryMock.Object,
            _useCaseRepositoryMock.Object,
            _projectReadRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _currentUserMock.Object,
            _userPermissionServiceMock.Object);

        // Act
        var act = async () => await handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ProjectAccessDeniedException>();
        _writeRepositoryMock.Verify(x => x.Remove(It.IsAny<Module>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_WhenModuleHasUseCases_ThrowsBusinessRuleViolationException()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var moduleId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var useCaseCount = 5;
        
        var project = ProjectBuilder.Create().Build();
        project.Id = projectId;
        
        var module = ModuleBuilder.Create().WithProjectId(projectId).Build();
        module.Id = moduleId;

        var command = new DeleteModuleCommand(moduleId);
        _currentUserMock.Setup(x => x.Id).Returns((UserId)userId);
        _readRepositoryMock.Setup(x => x.GetByIdAsync(moduleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(module);
        _projectReadRepositoryMock.Setup(x => x.GetByIdAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);
        _useCaseRepositoryMock.Setup(x => x.CountByModuleAsync(moduleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(useCaseCount);
        _userPermissionServiceMock.Setup(x => x.CanEditProject(It.IsAny<UserId>(), project))
            .Returns(true);

        var handler = new DeleteModuleCommandHandler(
            _readRepositoryMock.Object,
            _writeRepositoryMock.Object,
            _useCaseRepositoryMock.Object,
            _projectReadRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _currentUserMock.Object,
            _userPermissionServiceMock.Object);

        // Act
        var act = async () => await handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<BusinessRuleViolationException>()
            .WithMessage($"Modül içinde {useCaseCount} adet UseCase bulunmaktadır. " +
                        "Modül silindiğinde tüm UseCase'ler de silinecektir.");
        _writeRepositoryMock.Verify(x => x.Remove(It.IsAny<Module>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_ChecksUseCaseCountBeforeDeleting()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var moduleId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        
        var project = ProjectBuilder.Create().Build();
        project.Id = projectId;
        
        var module = ModuleBuilder.Create().WithProjectId(projectId).Build();
        module.Id = moduleId;

        var command = new DeleteModuleCommand(moduleId);
        _currentUserMock.Setup(x => x.Id).Returns((UserId)userId);
        _readRepositoryMock.Setup(x => x.GetByIdAsync(moduleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(module);
        _projectReadRepositoryMock.Setup(x => x.GetByIdAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);
        _useCaseRepositoryMock.Setup(x => x.CountByModuleAsync(moduleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);
        _userPermissionServiceMock.Setup(x => x.CanEditProject(It.IsAny<UserId>(), project))
            .Returns(true);
        _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var handler = new DeleteModuleCommandHandler(
            _readRepositoryMock.Object,
            _writeRepositoryMock.Object,
            _useCaseRepositoryMock.Object,
            _projectReadRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _currentUserMock.Object,
            _userPermissionServiceMock.Object);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        _useCaseRepositoryMock.Verify(
            x => x.CountByModuleAsync(moduleId, It.IsAny<CancellationToken>()), 
            Times.Once);
    }
}

