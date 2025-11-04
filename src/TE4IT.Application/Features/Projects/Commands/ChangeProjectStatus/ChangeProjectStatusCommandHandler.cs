using MediatR;
using TE4IT.Abstractions.Persistence.Repositories.Projects;
using TE4IT.Application.Abstractions.Persistence;

namespace TE4IT.Application.Features.Projects.Commands.ChangeProjectStatus;

public sealed class ChangeProjectStatusCommandHandler(
    IProjectReadRepository readRepository,
    IProjectWriteRepository writeRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<ChangeProjectStatusCommand, bool>
{
    public async Task<bool> Handle(ChangeProjectStatusCommand request, CancellationToken cancellationToken)
    {
        var project = await readRepository.GetByIdAsync(request.ProjectId, cancellationToken);
        if (project is null) return false;

        project.ChangeStatus(request.IsActive);
        writeRepository.Update(project, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}


