using MediatR;

namespace TE4IT.Application.Features.Tasks.Commands.CancelTask;

public sealed record CancelTaskCommand(Guid TaskId) : IRequest<bool>;

