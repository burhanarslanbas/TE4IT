using TE4IT.Domain.Enums;

namespace TE4IT.Application.Features.Tasks.Responses;

public sealed class TaskListItemResponse
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public TaskType TaskType { get; init; }
    public TaskState TaskState { get; init; }
    public Guid? AssigneeId { get; init; }
    public string? AssigneeName { get; init; }
    public DateTime? DueDate { get; init; }
    public bool IsOverdue { get; init; }
    public DateTime? StartedDate { get; init; }
}

