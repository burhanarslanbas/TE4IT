using MediatR;
using TE4IT.Abstractions.Persistence.Repositories.Projects;
using TE4IT.Application.Abstractions.Persistence;

namespace TE4IT.Application.Features.Projects.Commands.UpdateProject;

public sealed class UpdateProjectCommandHandler(
    IProjectReadRepository readRepository,
    IProjectWriteRepository writeRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<UpdateProjectCommand, bool>
{
    public async Task<bool> Handle(UpdateProjectCommand request, CancellationToken cancellationToken)
    {
        var project = await readRepository.GetByIdAsync(request.ProjectId, cancellationToken);
        if (project is null) return false;

        project.UpdateTitle(request.Title);
        project.UpdateDescription(request.Description);

        await writeRepository.UpdateAsync(project, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}


