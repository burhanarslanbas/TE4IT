using MediatR;
using TE4IT.Abstractions.Persistence.Repositories.Projects;
using TE4IT.Application.Features.Projects.Responses;

namespace TE4IT.Application.Features.Projects.Queries.GetProjectById;

public sealed class GetProjectByIdQueryHandler(IProjectReadRepository projectRepository) : IRequestHandler<GetProjectByIdQuery, ProjectResponse>
{
    public async Task<ProjectResponse> Handle(GetProjectByIdQuery request, CancellationToken cancellationToken)
    {
        var p = await projectRepository.GetByIdAsync(request.ProjectId, cancellationToken);
        if (p is null) throw new KeyNotFoundException("Project not found");
        return new ProjectResponse
        {
            Id = p.Id,
            Title = p.Title,
            Description = p.Description,
            IsActive = p.IsActive,
            StartedDate = p.StartedDate
        };
    }
}
