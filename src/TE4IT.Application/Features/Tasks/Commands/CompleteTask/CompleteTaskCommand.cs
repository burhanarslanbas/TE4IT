using MediatR;

namespace TE4IT.Application.Features.Tasks.Commands.CompleteTask;

public sealed record CompleteTaskCommand(Guid TaskId, string? CompletionNote = null) : IRequest<bool>;

