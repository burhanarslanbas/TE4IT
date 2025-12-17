using MediatR;
using TE4IT.Application.Abstractions.Auth;
using TE4IT.Application.Abstractions.Persistence.Repositories.Education.Enrollments;
using TE4IT.Application.Abstractions.Persistence.Repositories.Education.Progresses;
using TE4IT.Domain.Entities.Education;
using TE4IT.Domain.Exceptions.Common;

namespace TE4IT.Application.Features.Education.Progresses.Commands.UpdateProgress;

public sealed class UpdateProgressCommandHandler(
    IEnrollmentReadRepository enrollmentReadRepository,
    IProgressReadRepository progressReadRepository,
    IProgressWriteRepository progressWriteRepository,
    ICurrentUser currentUser) : IRequestHandler<UpdateProgressCommand, UpdateProgressCommandResponse>
{
    public async Task<UpdateProgressCommandResponse> Handle(UpdateProgressCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = currentUser.Id ?? throw new UnauthorizedAccessException("Kullanıcı kimliği bulunamadı.");

        // Enrollment kontrolü
        var enrollment = await enrollmentReadRepository.GetByUserAndCourseAsync(
            currentUserId.Value,
            request.CourseId,
            cancellationToken);

        if (enrollment is null)
        {
            throw new BusinessRuleViolationException("Bu kursa kayıtlı değilsiniz.");
        }

        // Mevcut progress kaydını bul veya oluştur
        var existingProgress = await progressReadRepository.GetByUserAndContentAsync(
            currentUserId.Value,
            request.ContentId,
            request.CourseId,
            cancellationToken);

        Progress progress;
        if (existingProgress is not null)
        {
            progress = existingProgress;
            if (request.WatchedPercentage.HasValue)
            {
                progress.UpdateWatchedPercentage(request.WatchedPercentage);
            }
            if (request.IsCompleted)
            {
                progress.MarkCompleted(request.TimeSpentMinutes, request.WatchedPercentage);
            }
            progressWriteRepository.Update(progress);
        }
        else
        {
            // TODO: StepId ve ContentId'den stepId bulunmalı (Course'dan)
            // Şimdilik basit bir yaklaşım kullanıyoruz
            progress = new Progress(
                currentUserId.Value,
                enrollment.Id,
                request.CourseId,
                Guid.Empty, // TODO: StepId bulunmalı
                request.ContentId);

            if (request.WatchedPercentage.HasValue)
            {
                progress.UpdateWatchedPercentage(request.WatchedPercentage);
            }
            if (request.IsCompleted)
            {
                progress.MarkCompleted(request.TimeSpentMinutes, request.WatchedPercentage);
            }

            await progressWriteRepository.AddAsync(progress, cancellationToken);
        }

        return new UpdateProgressCommandResponse { ProgressId = progress.Id };
    }
}

