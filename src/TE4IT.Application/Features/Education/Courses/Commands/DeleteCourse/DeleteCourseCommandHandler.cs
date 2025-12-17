using MediatR;
using TE4IT.Application.Abstractions.Auth;
using TE4IT.Application.Abstractions.Persistence.Repositories.Education.Courses;
using TE4IT.Domain.Constants;
using TE4IT.Domain.Exceptions.Common;

namespace TE4IT.Application.Features.Education.Courses.Commands.DeleteCourse;

public sealed class DeleteCourseCommandHandler(
    ICourseReadRepository courseReadRepository,
    ICourseWriteRepository courseWriteRepository,
    ICurrentUser currentUser) : IRequestHandler<DeleteCourseCommand, bool>
{
    public async Task<bool> Handle(DeleteCourseCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = currentUser.Id ?? throw new UnauthorizedAccessException("Kullanıcı kimliği bulunamadı.");

        // Yetki kontrolü: Sadece Admin
        if (!currentUser.IsInRole(RoleNames.Administrator))
        {
            throw new UnauthorizedAccessException("Kursu silmek için Admin yetkisi gereklidir.");
        }

        var course = await courseReadRepository.GetByIdAsync(request.CourseId, cancellationToken);
        if (course is null)
        {
            throw new ResourceNotFoundException("Course", request.CourseId);
        }

        await courseWriteRepository.SoftDeleteAsync(request.CourseId, cancellationToken);
        
        return true;
    }
}

