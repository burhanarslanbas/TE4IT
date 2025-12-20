using MediatR;

namespace TE4IT.Application.Features.Tasks.Commands.AssignAndStartTask;

public sealed record AssignAndStartTaskCommand(Guid TaskId, Guid AssigneeId) : IRequest<bool>;

