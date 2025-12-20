using MediatR;

namespace TE4IT.Application.Features.Tasks.Commands.StartTask;

public sealed record StartTaskCommand(Guid TaskId) : IRequest<bool>;

