using MediatR;

namespace TE4IT.Application.Features.Tasks.Commands.AssignTask;

public sealed record AssignTaskCommand(Guid TaskId, Guid AssigneeId) : IRequest<bool>;

