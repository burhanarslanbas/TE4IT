using MediatR;
using TE4IT.Domain.Enums;

namespace TE4IT.Application.Features.Tasks.Commands.CreateTask;

public sealed record CreateTaskCommand(
    Guid UseCaseId,
    string Title,
    TaskType TaskType,
    string? Description = null,
    string? ImportantNotes = null,
    DateTime? DueDate = null,
    Guid? AssigneeId = null) : IRequest<CreateTaskCommandResponse>;

