using MediatR;

namespace TE4IT.Application.Features.Tasks.Commands.CreateTask;

public sealed record CreateTaskCommand(
    Guid UseCaseId,
    Guid CreatorId,
    string Title,
    int TaskType,
    string? Description,
    string? ImportantNotes,
    DateTime? DueDate
) : IRequest<CreateTaskCommandResponse>;



