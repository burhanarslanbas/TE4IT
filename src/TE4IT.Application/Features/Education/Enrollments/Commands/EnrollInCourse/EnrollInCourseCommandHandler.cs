using MediatR;
using TE4IT.Application.Abstractions.Auth;
using TE4IT.Application.Abstractions.Persistence.Repositories.Education.Courses;
using TE4IT.Application.Abstractions.Persistence.Repositories.Education.Enrollments;
using TE4IT.Domain.Entities.Education;
using TE4IT.Domain.Exceptions.Common;
using TE4IT.Domain.Events;

namespace TE4IT.Application.Features.Education.Enrollments.Commands.EnrollInCourse;

public sealed class EnrollInCourseCommandHandler(
    ICourseReadRepository courseReadRepository,
    IEnrollmentReadRepository enrollmentReadRepository,
    IEnrollmentWriteRepository enrollmentWriteRepository,
    ICurrentUser currentUser) : IRequestHandler<EnrollInCourseCommand, EnrollInCourseCommandResponse>
{
    public async Task<EnrollInCourseCommandResponse> Handle(EnrollInCourseCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = currentUser.Id ?? throw new UnauthorizedAccessException("Kullanıcı kimliği bulunamadı.");

        // Course bulunur, IsActive kontrolü
        var course = await courseReadRepository.GetByIdAsync(request.CourseId, cancellationToken);
        if (course is null)
        {
            throw new ResourceNotFoundException("Course", request.CourseId);
        }

        if (!course.IsActive)
        {
            throw new BusinessRuleViolationException("Bu kurs aktif değildir.");
        }

        // Kullanıcının zaten kayıtlı olup olmadığı kontrol edilir
        var isEnrolled = await enrollmentReadRepository.IsEnrolledAsync(
            currentUserId.Value,
            request.CourseId,
            cancellationToken);

        if (isEnrolled)
        {
            throw new BusinessRuleViolationException("Bu kursa zaten kayıtlısınız.");
        }

        var enrollment = new Enrollment(currentUserId.Value, request.CourseId);
        
        // Domain event fırlat
        enrollment.AddDomainEvent(new EnrollmentCreatedEvent(enrollment.Id, currentUserId.Value, request.CourseId));

        await enrollmentWriteRepository.AddAsync(enrollment, cancellationToken);

        return new EnrollInCourseCommandResponse
        {
            EnrollmentId = enrollment.Id,
            EnrolledAt = enrollment.EnrolledAt
        };
    }
}

