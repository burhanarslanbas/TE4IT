namespace TE4IT.Application.Features.UseCases.Responses;

public sealed class UseCaseListItemResponse
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public bool IsActive { get; init; }
    public DateTime StartedDate { get; init; }
    public int TaskCount { get; init; }
}

