using MediatR;

namespace TE4IT.Application.Features.Projects.Commands.CreateProject;

public sealed record CreateProjectCommand(string Title, string? Description) : IRequest<CreateProjectCommandResponse>;
