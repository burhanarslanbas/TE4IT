using FluentAssertions;
using Moq;
using TE4IT.Abstractions.Persistence.Repositories.Modules;
using TE4IT.Abstractions.Persistence.Repositories.Projects;
using TE4IT.Abstractions.Persistence.Repositories.UseCases;
using TE4IT.Application.Abstractions.Auth;
using TE4IT.Application.Abstractions.Persistence;
using TE4IT.Application.Features.Modules.Commands.ChangeModuleStatus;
using TE4IT.Domain.Exceptions.Common;
using TE4IT.Domain.Exceptions.Projects;
using TE4IT.Domain.Services;
using TE4IT.Domain.ValueObjects;
using TE4IT.Tests.Unit.Common.Builders;
using Xunit;
using Module = TE4IT.Domain.Entities.Module;
using Project = TE4IT.Domain.Entities.Project;

namespace TE4IT.Tests.Unit.Application.Features.Modules.Commands.ChangeModuleStatus;

public class ChangeModuleStatusCommandHandlerTests
{
    private readonly Mock<IModuleReadRepository> _moduleReadRepositoryMock;
    private readonly Mock<IModuleWriteRepository> _moduleWriteRepositoryMock;
    private readonly Mock<IUseCaseWriteRepository> _useCaseWriteRepositoryMock;
    private readonly Mock<IProjectReadRepository> _projectReadRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ICurrentUser> _currentUserMock;
    private readonly Mock<IUserPermissionService> _userPermissionServiceMock;

    public ChangeModuleStatusCommandHandlerTests()
    {
        _moduleReadRepositoryMock = new Mock<IModuleReadRepository>();
        _moduleWriteRepositoryMock = new Mock<IModuleWriteRepository>();
        _useCaseWriteRepositoryMock = new Mock<IUseCaseWriteRepository>();
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
        
        var project = ProjectBuilder.Create()
            .WithIsActive(true)
            .Build();
        project.Id = projectId;
        
        var module = ModuleBuilder.Create()
            .WithProjectId(projectId)
            .WithIsActive(true)
            .Build();
        module.Id = moduleId;

        var command = new ChangeModuleStatusCommand(moduleId, false);
        _currentUserMock.Setup(x => x.Id).Returns((UserId)userId);
        _moduleReadRepositoryMock.Setup(x => x.GetByIdAsync(moduleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(module);
        _projectReadRepositoryMock.Setup(x => x.GetByIdAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);
        _userPermissionServiceMock.Setup(x => x.CanEditProject(It.IsAny<UserId>(), project))
            .Returns(true);
        _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var handler = new ChangeModuleStatusCommandHandler(
            _moduleReadRepositoryMock.Object,
            _moduleWriteRepositoryMock.Object,
            _useCaseWriteRepositoryMock.Object,
            _projectReadRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _currentUserMock.Object,
            _userPermissionServiceMock.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
        module.IsActive.Should().BeFalse();
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_WhenModuleNotFound_ReturnsFalse()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new ChangeModuleStatusCommand(Guid.NewGuid(), false);
        _currentUserMock.Setup(x => x.Id).Returns((UserId)userId);
        _moduleReadRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Module?)null);

        var handler = new ChangeModuleStatusCommandHandler(
            _moduleReadRepositoryMock.Object,
            _moduleWriteRepositoryMock.Object,
            _useCaseWriteRepositoryMock.Object,
            _projectReadRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _currentUserMock.Object,
            _userPermissionServiceMock.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeFalse();
        _moduleWriteRepositoryMock.Verify(x => x.Update(It.IsAny<Module>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_WhenUserCannotEditProject_ThrowsProjectAccessDeniedException()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var moduleId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        
        var project = ProjectBuilder.Create().WithIsActive(true).Build();
        project.Id = projectId;
        
        var module = ModuleBuilder.Create().WithProjectId(projectId).Build();
        module.Id = moduleId;

        var command = new ChangeModuleStatusCommand(moduleId, false);
        _currentUserMock.Setup(x => x.Id).Returns((UserId)userId);
        _moduleReadRepositoryMock.Setup(x => x.GetByIdAsync(moduleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(module);
        _projectReadRepositoryMock.Setup(x => x.GetByIdAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);
        _userPermissionServiceMock.Setup(x => x.CanEditProject(It.IsAny<UserId>(), project))
            .Returns(false);

        var handler = new ChangeModuleStatusCommandHandler(
            _moduleReadRepositoryMock.Object,
            _moduleWriteRepositoryMock.Object,
            _useCaseWriteRepositoryMock.Object,
            _projectReadRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _currentUserMock.Object,
            _userPermissionServiceMock.Object);

        // Act
        var act = async () => await handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ProjectAccessDeniedException>();
        _moduleWriteRepositoryMock.Verify(x => x.Update(It.IsAny<Module>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_WhenActivatingModuleInArchivedProject_ThrowsBusinessRuleViolationException()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var moduleId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        
        var project = ProjectBuilder.Create()
            .WithIsActive(false) // Proje arşivlenmiş
            .Build();
        project.Id = projectId;
        
        var module = ModuleBuilder.Create()
            .WithProjectId(projectId)
            .WithIsActive(false)
            .Build();
        module.Id = moduleId;

        var command = new ChangeModuleStatusCommand(moduleId, true); // Modülü aktif etmeye çalış
        _currentUserMock.Setup(x => x.Id).Returns((UserId)userId);
        _moduleReadRepositoryMock.Setup(x => x.GetByIdAsync(moduleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(module);
        _projectReadRepositoryMock.Setup(x => x.GetByIdAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);
        _userPermissionServiceMock.Setup(x => x.CanEditProject(It.IsAny<UserId>(), project))
            .Returns(true);

        var handler = new ChangeModuleStatusCommandHandler(
            _moduleReadRepositoryMock.Object,
            _moduleWriteRepositoryMock.Object,
            _useCaseWriteRepositoryMock.Object,
            _projectReadRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _currentUserMock.Object,
            _userPermissionServiceMock.Object);

        // Act
        var act = async () => await handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<BusinessRuleViolationException>()
            .WithMessage("Arşivlenmiş projede modül aktif edilemez. Önce projeyi aktif edin.");
        _moduleWriteRepositoryMock.Verify(x => x.Update(It.IsAny<Module>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_WhenArchivingModule_CallsArchiveByModuleIdAsync()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var moduleId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        
        var project = ProjectBuilder.Create().WithIsActive(true).Build();
        project.Id = projectId;
        
        var module = ModuleBuilder.Create()
            .WithProjectId(projectId)
            .WithIsActive(true)
            .Build();
        module.Id = moduleId;

        var command = new ChangeModuleStatusCommand(moduleId, false); // Arşivle
        _currentUserMock.Setup(x => x.Id).Returns((UserId)userId);
        _moduleReadRepositoryMock.Setup(x => x.GetByIdAsync(moduleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(module);
        _projectReadRepositoryMock.Setup(x => x.GetByIdAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);
        _userPermissionServiceMock.Setup(x => x.CanEditProject(It.IsAny<UserId>(), project))
            .Returns(true);
        _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var handler = new ChangeModuleStatusCommandHandler(
            _moduleReadRepositoryMock.Object,
            _moduleWriteRepositoryMock.Object,
            _useCaseWriteRepositoryMock.Object,
            _projectReadRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _currentUserMock.Object,
            _userPermissionServiceMock.Object);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        module.IsActive.Should().BeFalse();
        _useCaseWriteRepositoryMock.Verify(
            x => x.ArchiveByModuleIdAsync(moduleId, It.IsAny<CancellationToken>()), 
            Times.Once);
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_WhenActivatingModule_DoesNotCallArchiveByModuleIdAsync()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var moduleId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        
        var project = ProjectBuilder.Create().WithIsActive(true).Build();
        project.Id = projectId;
        
        var module = ModuleBuilder.Create()
            .WithProjectId(projectId)
            .WithIsActive(false)
            .Build();
        module.Id = moduleId;

        var command = new ChangeModuleStatusCommand(moduleId, true); // Aktif et
        _currentUserMock.Setup(x => x.Id).Returns((UserId)userId);
        _moduleReadRepositoryMock.Setup(x => x.GetByIdAsync(moduleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(module);
        _projectReadRepositoryMock.Setup(x => x.GetByIdAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);
        _userPermissionServiceMock.Setup(x => x.CanEditProject(It.IsAny<UserId>(), project))
            .Returns(true);
        _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var handler = new ChangeModuleStatusCommandHandler(
            _moduleReadRepositoryMock.Object,
            _moduleWriteRepositoryMock.Object,
            _useCaseWriteRepositoryMock.Object,
            _projectReadRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _currentUserMock.Object,
            _userPermissionServiceMock.Object);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        module.IsActive.Should().BeTrue();
        _useCaseWriteRepositoryMock.Verify(
            x => x.ArchiveByModuleIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), 
            Times.Never);
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_CallsRepositoryUpdate()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var moduleId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        
        var project = ProjectBuilder.Create().Build();
        project.Id = projectId;
        
        var module = ModuleBuilder.Create().WithProjectId(projectId).Build();
        module.Id = moduleId;

        var command = new ChangeModuleStatusCommand(moduleId, false);
        _currentUserMock.Setup(x => x.Id).Returns((UserId)userId);
        _moduleReadRepositoryMock.Setup(x => x.GetByIdAsync(moduleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(module);
        _projectReadRepositoryMock.Setup(x => x.GetByIdAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);
        _userPermissionServiceMock.Setup(x => x.CanEditProject(It.IsAny<UserId>(), project))
            .Returns(true);
        _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var handler = new ChangeModuleStatusCommandHandler(
            _moduleReadRepositoryMock.Object,
            _moduleWriteRepositoryMock.Object,
            _useCaseWriteRepositoryMock.Object,
            _projectReadRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _currentUserMock.Object,
            _userPermissionServiceMock.Object);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        _moduleWriteRepositoryMock.Verify(
            x => x.Update(It.Is<Module>(m => m.Id == moduleId), It.IsAny<CancellationToken>()), 
            Times.Once);
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_CallsUnitOfWorkSaveChangesAsync()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var moduleId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        
        var project = ProjectBuilder.Create().Build();
        project.Id = projectId;
        
        var module = ModuleBuilder.Create().WithProjectId(projectId).Build();
        module.Id = moduleId;

        var command = new ChangeModuleStatusCommand(moduleId, false);
        _currentUserMock.Setup(x => x.Id).Returns((UserId)userId);
        _moduleReadRepositoryMock.Setup(x => x.GetByIdAsync(moduleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(module);
        _projectReadRepositoryMock.Setup(x => x.GetByIdAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);
        _userPermissionServiceMock.Setup(x => x.CanEditProject(It.IsAny<UserId>(), project))
            .Returns(true);
        _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var handler = new ChangeModuleStatusCommandHandler(
            _moduleReadRepositoryMock.Object,
            _moduleWriteRepositoryMock.Object,
            _useCaseWriteRepositoryMock.Object,
            _projectReadRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _currentUserMock.Object,
            _userPermissionServiceMock.Object);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}

