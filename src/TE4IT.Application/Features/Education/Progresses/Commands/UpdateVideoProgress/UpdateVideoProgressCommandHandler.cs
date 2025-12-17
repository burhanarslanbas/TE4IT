using MediatR;
using TE4IT.Application.Abstractions.Auth;
using TE4IT.Application.Abstractions.Persistence.Repositories.Education.Progresses;
using TE4IT.Domain.Entities.Education;
using TE4IT.Domain.Exceptions.Common;

namespace TE4IT.Application.Features.Education.Progresses.Commands.UpdateVideoProgress;

public sealed class UpdateVideoProgressCommandHandler(
    IProgressReadRepository progressReadRepository,
    IProgressWriteRepository progressWriteRepository,
    ICurrentUser currentUser) : IRequestHandler<UpdateVideoProgressCommand, UpdateVideoProgressCommandResponse>
{
    public async Task<UpdateVideoProgressCommandResponse> Handle(UpdateVideoProgressCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = currentUser.Id ?? throw new UnauthorizedAccessException("Kullanıcı kimliği bulunamadı.");

        // Progress kaydı güncellenir
        var existingProgress = await progressReadRepository.GetByUserAndContentAsync(
            currentUserId.Value,
            request.ContentId,
            request.CourseId,
            cancellationToken);

        Progress progress;
        if (existingProgress is not null)
        {
            progress = existingProgress;
            progress.UpdateWatchedPercentage(request.WatchedPercentage);
            if (request.IsCompleted)
            {
                var timeSpentMinutes = request.TimeSpentSeconds / 60;
                progress.MarkCompleted(timeSpentMinutes, request.WatchedPercentage);
            }
            progressWriteRepository.Update(progress);
        }
        else
        {
            // TODO: Enrollment ve StepId bulunmalı
            throw new BusinessRuleViolationException("Progress kaydı bulunamadı. Önce içeriğe erişim sağlanmalıdır.");
        }

        return new UpdateVideoProgressCommandResponse { ProgressId = progress.Id };
    }
}

