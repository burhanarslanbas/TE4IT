using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using TE4IT.Abstractions.Persistence.Repositories.Projects;
using TE4IT.Application.Abstractions.Auth;
using TE4IT.Application.Abstractions.Persistence;
using TE4IT.Application.Features.Projects.Commands.CreateProject;
using TE4IT.Domain.Exceptions.Common;
using Project = TE4IT.Domain.Entities.Project;
using TE4IT.Domain.ValueObjects;
using TE4IT.Tests.Unit.Common.Builders;
using Xunit;

namespace TE4IT.Tests.Unit.Application.Features.Projects.Commands.CreateProject;

public class CreateProjectCommandHandlerTests
{
    private readonly Mock<IProjectWriteRepository> _writeRepositoryMock;
    private readonly Mock<IProjectReadRepository> _readRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly CreateProjectCommandHandler _handler;

    public CreateProjectCommandHandlerTests()
    {
        _writeRepositoryMock = new Mock<IProjectWriteRepository>();
        _readRepositoryMock = new Mock<IProjectReadRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _handler = new CreateProjectCommandHandler(
            _writeRepositoryMock.Object,
            _readRepositoryMock.Object,
            _unitOfWorkMock.Object,
            CurrentUserBuilder.Create().Build());
    }

    [Fact]
    public async Task Handle_WithValidCommand_ReturnsProjectId()
    {
        // Arrange
        var currentUser = CurrentUserBuilder.Create()
            .AsTeamLead()
            .WithId(Guid.NewGuid())
            .Build();
        var handler = new CreateProjectCommandHandler(
            _writeRepositoryMock.Object,
            _readRepositoryMock.Object,
            _unitOfWorkMock.Object,
            currentUser);

        var command = new CreateProjectCommand("Test Project", "Test Description");
        _readRepositoryMock.Setup(x => x.CountByCreatorAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);
        _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();
        _writeRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Project>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenCurrentUserIsNull_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var currentUser = CurrentUserBuilder.Create()
            .WithNoId()
            .Build();
        var handler = new CreateProjectCommandHandler(
            _writeRepositoryMock.Object,
            _readRepositoryMock.Object,
            _unitOfWorkMock.Object,
            currentUser);

        var command = new CreateProjectCommand("Test Project", "Test Description");

        // Act
        var act = async () => await handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>();
        _writeRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Project>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenTrialUserHasNoProjects_CreatesProject()
    {
        // Arrange
        var currentUser = CurrentUserBuilder.Create()
            .AsTrial()
            .WithId(Guid.NewGuid())
            .Build();
        var handler = new CreateProjectCommandHandler(
            _writeRepositoryMock.Object,
            _readRepositoryMock.Object,
            _unitOfWorkMock.Object,
            currentUser);

        var command = new CreateProjectCommand("Test Project", "Test Description");
        _readRepositoryMock.Setup(x => x.CountByCreatorAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);
        _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();
        _readRepositoryMock.Verify(x => x.CountByCreatorAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
        _writeRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Project>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenTrialUserHasOneProject_ThrowsBusinessRuleViolationException()
    {
        // Arrange
        var currentUser = CurrentUserBuilder.Create()
            .AsTrial()
            .WithId(Guid.NewGuid())
            .Build();
        var handler = new CreateProjectCommandHandler(
            _writeRepositoryMock.Object,
            _readRepositoryMock.Object,
            _unitOfWorkMock.Object,
            currentUser);

        var command = new CreateProjectCommand("Test Project", "Test Description");
        _readRepositoryMock.Setup(x => x.CountByCreatorAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var act = async () => await handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<BusinessRuleViolationException>()
            .WithMessage("Trial kullanıcı en fazla 1 proje oluşturabilir.");
        _writeRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Project>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenNonTrialUser_CreatesProjectWithoutQuotaCheck()
    {
        // Arrange
        var currentUser = CurrentUserBuilder.Create()
            .AsAdministrator()
            .WithId(Guid.NewGuid())
            .Build();
        var handler = new CreateProjectCommandHandler(
            _writeRepositoryMock.Object,
            _readRepositoryMock.Object,
            _unitOfWorkMock.Object,
            currentUser);

        var command = new CreateProjectCommand("Test Project", "Test Description");
        _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();
        _readRepositoryMock.Verify(x => x.CountByCreatorAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _writeRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Project>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_CallsRepositoryAddAsync()
    {
        // Arrange
        var currentUser = CurrentUserBuilder.Create()
            .AsTeamLead()
            .WithId(Guid.NewGuid())
            .Build();
        var handler = new CreateProjectCommandHandler(
            _writeRepositoryMock.Object,
            _readRepositoryMock.Object,
            _unitOfWorkMock.Object,
            currentUser);

        var command = new CreateProjectCommand("Test Project", "Test Description");
        _readRepositoryMock.Setup(x => x.CountByCreatorAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);
        _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        _writeRepositoryMock.Verify(x => x.AddAsync(It.Is<Project>(p => 
            p.Title == command.Title && 
            p.Description == command.Description &&
            p.CreatorId == currentUser.Id), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_CallsUnitOfWorkSaveChangesAsync()
    {
        // Arrange
        var currentUser = CurrentUserBuilder.Create()
            .AsTeamLead()
            .WithId(Guid.NewGuid())
            .Build();
        var handler = new CreateProjectCommandHandler(
            _writeRepositoryMock.Object,
            _readRepositoryMock.Object,
            _unitOfWorkMock.Object,
            currentUser);

        var command = new CreateProjectCommand("Test Project", "Test Description");
        _readRepositoryMock.Setup(x => x.CountByCreatorAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);
        _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}

