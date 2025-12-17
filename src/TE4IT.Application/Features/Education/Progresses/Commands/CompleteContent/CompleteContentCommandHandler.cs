using MediatR;
using TE4IT.Application.Abstractions.Auth;
using TE4IT.Application.Abstractions.Persistence.Repositories.Education.Courses;
using TE4IT.Application.Abstractions.Persistence.Repositories.Education.Enrollments;
using TE4IT.Application.Abstractions.Persistence.Repositories.Education.Progresses;
using TE4IT.Domain.Entities.Education;
using TE4IT.Domain.Exceptions.Common;
using TE4IT.Domain.Events;
using TE4IT.Domain.Services;

namespace TE4IT.Application.Features.Education.Progresses.Commands.CompleteContent;

public sealed class CompleteContentCommandHandler(
    ICourseReadRepository courseReadRepository,
    IEnrollmentReadRepository enrollmentReadRepository,
    IEnrollmentWriteRepository enrollmentWriteRepository,
    IProgressReadRepository progressReadRepository,
    IProgressWriteRepository progressWriteRepository,
    ICourseProgressService courseProgressService,
    ICurrentUser currentUser) : IRequestHandler<CompleteContentCommand, CompleteContentCommandResponse>
{
    public async Task<CompleteContentCommandResponse> Handle(CompleteContentCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = currentUser.Id ?? throw new UnauthorizedAccessException("Kullanıcı kimliği bulunamadı.");

        // Course + Content bulunur (embedded document içinden)
        var course = await courseReadRepository.GetByIdAsync(request.CourseId, cancellationToken);
        if (course is null)
        {
            throw new ResourceNotFoundException("Course", request.CourseId);
        }

        // Content'i bul
        var content = course.Roadmap?.Steps
            .SelectMany(s => s.Contents)
            .FirstOrDefault(c => c.Id == request.ContentId);

        if (content is null)
        {
            throw new ResourceNotFoundException("Content", request.ContentId);
        }

        // Step'i bul
        var step = course.Roadmap?.Steps
            .FirstOrDefault(s => s.Contents.Any(c => c.Id == request.ContentId));

        if (step is null)
        {
            throw new ResourceNotFoundException("Step", request.ContentId);
        }

        // Enrollment kontrolü
        var enrollment = await enrollmentReadRepository.GetByUserAndCourseAsync(
            currentUserId.Value,
            request.CourseId,
            cancellationToken);

        if (enrollment is null)
        {
            throw new BusinessRuleViolationException("Bu kursa kayıtlı değilsiniz.");
        }

        // İlk içerik erişiminde StartedAt set et
        if (enrollment.StartedAt is null)
        {
            enrollment.MarkStarted();
            enrollmentWriteRepository.Update(enrollment);
        }

        // Adım kilitleme kontrolü
        var canAccess = await courseProgressService.CanAccessStepAsync(
            currentUserId.Value,
            step.Id,
            cancellationToken);

        if (!canAccess)
        {
            throw new BusinessRuleViolationException("Bu adıma erişim yetkiniz bulunmamaktadır. Önceki adımları tamamlamanız gerekmektedir.");
        }

        // Progress kaydı oluşturulur/güncellenir
        var existingProgress = await progressReadRepository.GetByUserAndContentAsync(
            currentUserId.Value,
            request.ContentId,
            request.CourseId,
            cancellationToken);

        Progress progress;
        if (existingProgress is not null)
        {
            progress = existingProgress;
            progress.MarkCompleted(request.TimeSpentMinutes, request.WatchedPercentage);
            progress.AddDomainEvent(new ContentCompletedEvent(progress.Id, currentUserId.Value, request.ContentId));
            progressWriteRepository.Update(progress);
        }
        else
        {
            progress = new Progress(
                currentUserId.Value,
                enrollment.Id,
                request.CourseId,
                step.Id,
                request.ContentId);

            progress.MarkCompleted(request.TimeSpentMinutes, request.WatchedPercentage);
            progress.AddDomainEvent(new ContentCompletedEvent(progress.Id, currentUserId.Value, request.ContentId));
            await progressWriteRepository.AddAsync(progress, cancellationToken);
        }

        // Adım tamamlanma kontrolü
        var isStepCompleted = await courseProgressService.IsStepCompletedAsync(
            currentUserId.Value,
            step.Id,
            cancellationToken);

        bool isCourseCompleted = false;
        if (isStepCompleted)
        {
            // Kurs tamamlanma kontrolü
            isCourseCompleted = await courseProgressService.IsCourseCompletedAsync(
                currentUserId.Value,
                request.CourseId,
                cancellationToken);
            
            // Kurs tamamlandığında Enrollment.CompletedAt set et ve event fırlat
            if (isCourseCompleted)
            {
                enrollment.MarkCompleted();
                enrollment.AddDomainEvent(new CourseCompletedEvent(enrollment.Id, currentUserId.Value, request.CourseId));
                enrollmentWriteRepository.Update(enrollment);
            }
        }

        return new CompleteContentCommandResponse
        {
            ProgressId = progress.Id,
            IsStepCompleted = isStepCompleted,
            IsCourseCompleted = isCourseCompleted
        };
    }
}

