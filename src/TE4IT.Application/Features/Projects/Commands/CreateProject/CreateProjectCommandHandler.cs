using MediatR;
using TE4IT.Abstractions.Persistence.Repositories.Projects;
using TE4IT.Application.Abstractions.Auth;
using TE4IT.Application.Abstractions.Persistence;
using TE4IT.Domain.Entities;

namespace TE4IT.Application.Features.Projects.Commands.CreateProject;

public sealed class CreateProjectCommandHandler(
    IProjectWriteRepository projectRepository,
    IProjectReadRepository projectReadRepository,
    IUnitOfWork unitOfWork,
    ICurrentUser currentUser) : IRequestHandler<CreateProjectCommand, CreateProjectCommandResponse>
{
    public async Task<CreateProjectCommandResponse> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
    {
        var creatorId = currentUser.Id ?? throw new UnauthorizedAccessException();
        var isTrial = currentUser.Roles.Contains(TE4IT.Domain.Constants.RoleNames.Trial);
        if (isTrial)
        {
            var count = await projectReadRepository.CountByCreatorAsync(creatorId.Value, cancellationToken);
            if (count >= 1)
                throw new ArgumentException("Trial kullanıcı en fazla 1 proje oluşturabilir.");
        }
        var project = Project.Create(creatorId, request.Title, request.Description);
        await projectRepository.AddAsync(project, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreateProjectCommandResponse { Id = project.Id };
    }
}