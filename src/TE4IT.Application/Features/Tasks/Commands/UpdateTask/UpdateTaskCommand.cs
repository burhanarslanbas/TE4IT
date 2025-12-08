using MediatR;

namespace TE4IT.Application.Features.Tasks.Commands.UpdateTask;

public sealed record UpdateTaskCommand(
    Guid TaskId,
    string Title,
    string? Description = null,
    string? ImportantNotes = null,
    DateTime? DueDate = null) : IRequest<bool>;

