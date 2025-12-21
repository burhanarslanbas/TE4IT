using MediatR;
using TE4IT.Abstractions.Persistence.Repositories.Projects;
using TE4IT.Application.Abstractions.Auth;
using TE4IT.Application.Features.Projects.Responses;
using TE4IT.Domain.Exceptions.Common;
using TE4IT.Domain.Exceptions.Projects;
using TE4IT.Domain.Services;

namespace TE4IT.Application.Features.Projects.Queries.GetProjectById;

public sealed class GetProjectByIdQueryHandler(
    IProjectReadRepository projectRepository,
    ICurrentUser currentUser,
    IUserPermissionService userPermissionService) : IRequestHandler<GetProjectByIdQuery, ProjectResponse>
{
    public async Task<ProjectResponse> Handle(GetProjectByIdQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = currentUser.Id ?? throw new UnauthorizedAccessException();

        var project = await projectRepository.GetByIdAsync(request.Id, cancellationToken);
        if (project is null)
            throw new ResourceNotFoundException("Project", request.Id);

        // Erişim kontrolü
        if (!userPermissionService.CanAccessProject(currentUserId, project))
            throw new ProjectAccessDeniedException(request.Id, currentUserId.Value, "Projeye erişim yetkiniz bulunmamaktadır.");

        return new ProjectResponse
        {
            Id = project.Id,
            Title = project.Title,
            Description = project.Description,
            IsActive = project.IsActive,
            StartedDate = project.StartedDate
        };
    }
}
