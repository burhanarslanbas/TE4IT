namespace TE4IT.Application.Features.Modules.Responses;

public sealed class ModuleListItemResponse
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public bool IsActive { get; init; }
    public DateTime StartedDate { get; init; }
    public int UseCaseCount { get; init; }
}

