using MediatR;
using TE4IT.Abstractions.Persistence.Repositories.Projects;
using TE4IT.Application.Abstractions.Auth;
using TE4IT.Application.Abstractions.Persistence;
using TE4IT.Domain.Entities;

namespace TE4IT.Application.Features.Projects.Commands.CreateProject;

public sealed class CreateProjectCommandHandler(
    IProjectWriteRepository projectRepository,
    IUnitOfWork unitOfWork,
    IPolicyAuthorizer authorizer) : IRequestHandler<CreateProjectCommand, CreateProjectCommandResponse>
{
    public async Task<CreateProjectCommandResponse> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
    {
        var ok = await authorizer.AuthorizeAsync("ProjectCreate", cancellationToken);
        if (!ok) throw new UnauthorizedAccessException();

        var project = Project.Create(request.CreatorId,request.Title,request.Description);
        await projectRepository.AddAsync(project, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreateProjectCommandResponse { Id = project.Id };
    }
}