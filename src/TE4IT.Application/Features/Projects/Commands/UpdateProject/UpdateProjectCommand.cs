using MediatR;

namespace TE4IT.Application.Features.Projects.Commands.UpdateProject;

public sealed record UpdateProjectCommand(Guid ProjectId, string Title, string? Description) : IRequest<bool>;


