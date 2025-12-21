using MediatR;

namespace TE4IT.Application.Features.Tasks.Commands.UnassignTask;

public sealed record UnassignTaskCommand(Guid TaskId) : IRequest<bool>;

