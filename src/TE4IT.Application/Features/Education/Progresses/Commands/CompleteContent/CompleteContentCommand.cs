using MediatR;

namespace TE4IT.Application.Features.Education.Progresses.Commands.CompleteContent;

public sealed record CompleteContentCommand(
    Guid ContentId,
    Guid CourseId,
    int? TimeSpentMinutes = null,
    int? WatchedPercentage = null) : IRequest<CompleteContentCommandResponse>;

