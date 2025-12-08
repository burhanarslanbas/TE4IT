namespace TE4IT.Application.Features.Modules.Responses;

public sealed class ModuleResponse
{
    public Guid Id { get; init; }
    public Guid ProjectId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public bool IsActive { get; init; }
    public DateTime StartedDate { get; init; }
}

