using TE4IT.Domain.Enums;

namespace TE4IT.Application.Features.Tasks.Responses;

public sealed class TaskResponse
{
    public Guid Id { get; init; }
    public Guid UseCaseId { get; init; }
    public Guid CreatorId { get; init; }
    public Guid? AssigneeId { get; init; }
    public string? AssigneeName { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string? ImportantNotes { get; init; }
    public DateTime? StartedDate { get; init; }
    public DateTime? DueDate { get; init; }
    public TaskType TaskType { get; init; }
    public TaskState TaskState { get; init; }
    public List<TaskRelationResponse> Relations { get; init; } = new();
}

