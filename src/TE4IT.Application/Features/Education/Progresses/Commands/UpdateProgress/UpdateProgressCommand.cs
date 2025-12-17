using MediatR;

namespace TE4IT.Application.Features.Education.Progresses.Commands.UpdateProgress;

public sealed record UpdateProgressCommand(
    Guid ContentId,
    Guid CourseId,
    int? TimeSpentMinutes = null,
    int? WatchedPercentage = null,
    bool IsCompleted = false) : IRequest<UpdateProgressCommandResponse>;

