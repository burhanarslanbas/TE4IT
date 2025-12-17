using MediatR;

namespace TE4IT.Application.Features.Education.Progresses.Commands.UpdateVideoProgress;

public sealed record UpdateVideoProgressCommand(
    Guid ContentId,
    Guid CourseId,
    int WatchedPercentage,
    int TimeSpentSeconds,
    bool IsCompleted = false) : IRequest<UpdateVideoProgressCommandResponse>;

