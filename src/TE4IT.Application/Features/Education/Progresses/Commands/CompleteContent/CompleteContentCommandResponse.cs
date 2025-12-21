namespace TE4IT.Application.Features.Education.Progresses.Commands.CompleteContent;

public sealed class CompleteContentCommandResponse
{
    public Guid ProgressId { get; init; }
    public bool IsStepCompleted { get; init; }
    public bool IsCourseCompleted { get; init; }
}

