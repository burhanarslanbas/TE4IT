namespace TE4IT.Application.Features.Education.Roadmaps.Responses;

/// <summary>
/// Roadmap response DTO
/// </summary>
public sealed class RoadmapResponse
{
    public string Title { get; init; } = default!;
    public string? Description { get; init; }
    public int EstimatedDurationMinutes { get; init; }
    public IReadOnlyList<StepResponse> Steps { get; init; } = Array.Empty<StepResponse>();
}

