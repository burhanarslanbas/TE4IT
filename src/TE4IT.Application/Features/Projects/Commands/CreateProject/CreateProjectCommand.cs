using MediatR;
using TE4IT.Domain.ValueObjects;

namespace TE4IT.Application.Features.Projects.Commands.CreateProject;

public sealed record CreateProjectCommand(UserId CreatorId, string Title, string? Description) : IRequest<CreateProjectCommandResponse>;
