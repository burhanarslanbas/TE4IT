using TE4IT.Domain.Enums;

namespace TE4IT.Application.Features.Tasks.Responses;

public sealed class TaskRelationResponse
{
    public Guid Id { get; init; }
    public Guid TargetTaskId { get; init; }
    public TaskRelationType RelationType { get; init; }
    public string TargetTaskTitle { get; init; } = string.Empty;
}

