using MediatR;

namespace TE4IT.Application.Features.Projects.Commands.ChangeProjectStatus;

public sealed record ChangeProjectStatusCommand(Guid ProjectId, bool IsActive) : IRequest<bool>;


