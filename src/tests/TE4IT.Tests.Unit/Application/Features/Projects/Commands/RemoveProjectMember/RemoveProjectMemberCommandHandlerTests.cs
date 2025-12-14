using FluentAssertions;
using Moq;
using TE4IT.Abstractions.Persistence.Repositories.ProjectMembers;
using TE4IT.Abstractions.Persistence.Repositories.Projects;
using TE4IT.Application.Abstractions.Auth;
using TE4IT.Application.Abstractions.Persistence;
using TE4IT.Application.Features.Projects.Commands.RemoveProjectMember;
using TE4IT.Domain.Entities;
using TE4IT.Domain.Enums;
using TE4IT.Domain.Exceptions.Common;
using TE4IT.Domain.Exceptions.Projects;
using TE4IT.Domain.Services;
using TE4IT.Domain.ValueObjects;
using Xunit;

namespace TE4IT.Tests.Unit.Application.Features.Projects.Commands.RemoveProjectMember;

public class RemoveProjectMemberCommandHandlerTests
{
    private readonly Mock<IProjectReadRepository> _projectRepository;
    private readonly Mock<IProjectMemberReadRepository> _projectMemberReadRepository;
    private readonly Mock<IProjectMemberWriteRepository> _projectMemberWriteRepository;
    private readonly Mock<IUnitOfWork> _unitOfWork;
    private readonly Mock<ICurrentUser> _currentUser;
    private readonly Mock<IUserPermissionService> _userPermissionService;
    private readonly RemoveProjectMemberCommandHandler _handler;

    public RemoveProjectMemberCommandHandlerTests()
    {
        _projectRepository = new Mock<IProjectReadRepository>();
        _projectMemberReadRepository = new Mock<IProjectMemberReadRepository>();
        _projectMemberWriteRepository = new Mock<IProjectMemberWriteRepository>();
        _unitOfWork = new Mock<IUnitOfWork>();
        _currentUser = new Mock<ICurrentUser>();
        _userPermissionService = new Mock<IUserPermissionService>();

        _handler = new RemoveProjectMemberCommandHandler(
            _projectRepository.Object,
            _projectMemberReadRepository.Object,
            _projectMemberWriteRepository.Object,
            _unitOfWork.Object,
            _currentUser.Object,
            _userPermissionService.Object);
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_AsOwner_RemovesMemberSuccessfully()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var ownerId = Guid.NewGuid();
        var memberId = Guid.NewGuid();

        var project = Project.Create((UserId)ownerId, "Test Project", "Description");
        var memberToRemove = ProjectMember.Create(projectId, (UserId)memberId, ProjectRole.Member);

        _currentUser.Setup(x => x.Id).Returns((UserId)ownerId);
        _projectRepository.Setup(x => x.GetByIdAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);
        _userPermissionService.Setup(x => x.IsSystemAdministrator(It.IsAny<UserId>())).Returns(false);
        _userPermissionService.Setup(x => x.GetUserProjectRole(It.IsAny<UserId>(), project))
            .Returns(ProjectRole.Owner);
        _projectMemberReadRepository.Setup(x => x.GetByProjectIdAndUserIdAsync(projectId, memberId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(memberToRemove);

        var command = new RemoveProjectMemberCommand(projectId, memberId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
        _projectMemberWriteRepository.Verify(x => x.Remove(memberToRemove, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_RemoveLastOwner_ThrowsException()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var ownerId = Guid.NewGuid();
        var targetOwnerId = Guid.NewGuid();

        var project = Project.Create((UserId)ownerId, "Test Project", "Description");
        var ownerToRemove = ProjectMember.Create(projectId, (UserId)targetOwnerId, ProjectRole.Owner);

        _currentUser.Setup(x => x.Id).Returns((UserId)ownerId);
        _projectRepository.Setup(x => x.GetByIdAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);
        _userPermissionService.Setup(x => x.IsSystemAdministrator(It.IsAny<UserId>())).Returns(false);
        _userPermissionService.Setup(x => x.GetUserProjectRole(It.IsAny<UserId>(), project))
            .Returns(ProjectRole.Owner);
        _projectMemberReadRepository.Setup(x => x.GetByProjectIdAndUserIdAsync(projectId, targetOwnerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ownerToRemove);
        _projectMemberReadRepository.Setup(x => x.CountByProjectIdAndRoleAsync(projectId, ProjectRole.Owner, It.IsAny<CancellationToken>()))
            .ReturnsAsync(1); // SON OWNER!

        var command = new RemoveProjectMemberCommand(projectId, targetOwnerId);

        // Act & Assert
        await Assert.ThrowsAsync<BusinessRuleViolationException>(
            async () => await _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_RemoveOneOfMultipleOwners_Succeeds()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var owner1Id = Guid.NewGuid();
        var owner2Id = Guid.NewGuid();

        var project = Project.Create((UserId)owner1Id, "Test Project", "Description");
        var ownerToRemove = ProjectMember.Create(projectId, (UserId)owner2Id, ProjectRole.Owner);

        _currentUser.Setup(x => x.Id).Returns((UserId)owner1Id);
        _projectRepository.Setup(x => x.GetByIdAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);
        _userPermissionService.Setup(x => x.IsSystemAdministrator(It.IsAny<UserId>())).Returns(false);
        _userPermissionService.Setup(x => x.GetUserProjectRole(It.IsAny<UserId>(), project))
            .Returns(ProjectRole.Owner);
        _projectMemberReadRepository.Setup(x => x.GetByProjectIdAndUserIdAsync(projectId, owner2Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ownerToRemove);
        _projectMemberReadRepository.Setup(x => x.CountByProjectIdAndRoleAsync(projectId, ProjectRole.Owner, It.IsAny<CancellationToken>()))
            .ReturnsAsync(2); // 2 OWNER VAR, çıkarılabilir!

        var command = new RemoveProjectMemberCommand(projectId, owner2Id);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
        _projectMemberWriteRepository.Verify(x => x.Remove(ownerToRemove, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_UserTriesToRemoveThemselves_ThrowsException()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var project = Project.Create((UserId)userId, "Test Project", "Description");
        var member = ProjectMember.Create(projectId, (UserId)userId, ProjectRole.Owner);

        _currentUser.Setup(x => x.Id).Returns((UserId)userId);
        _projectRepository.Setup(x => x.GetByIdAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);
        _userPermissionService.Setup(x => x.IsSystemAdministrator(It.IsAny<UserId>())).Returns(false);
        _userPermissionService.Setup(x => x.GetUserProjectRole(It.IsAny<UserId>(), project))
            .Returns(ProjectRole.Owner);
        _projectMemberReadRepository.Setup(x => x.GetByProjectIdAndUserIdAsync(projectId, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(member);

        var command = new RemoveProjectMemberCommand(projectId, userId);

        // Act & Assert
        await Assert.ThrowsAsync<BusinessRuleViolationException>(
            async () => await _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_AsMember_ThrowsForbiddenException()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var memberId = Guid.NewGuid();
        var targetUserId = Guid.NewGuid();

        var project = Project.Create((UserId)Guid.NewGuid(), "Test Project", "Description");

        _currentUser.Setup(x => x.Id).Returns((UserId)memberId);
        _projectRepository.Setup(x => x.GetByIdAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);
        _userPermissionService.Setup(x => x.IsSystemAdministrator(It.IsAny<UserId>())).Returns(false);
        _userPermissionService.Setup(x => x.GetUserProjectRole(It.IsAny<UserId>(), project))
            .Returns(ProjectRole.Member); // Member rolü

        var command = new RemoveProjectMemberCommand(projectId, targetUserId);

        // Act & Assert
        await Assert.ThrowsAsync<ProjectAccessDeniedException>(
            async () => await _handler.Handle(command, CancellationToken.None));
    }
}
