using MediatR;
using TE4IT.Application.Abstractions.Auth;
using TE4IT.Application.Abstractions.Persistence.Repositories.Education.Courses;
using TE4IT.Domain.Constants;
using TE4IT.Domain.Entities.Education;
using TE4IT.Domain.Exceptions.Common;

namespace TE4IT.Application.Features.Education.Courses.Commands.UpdateCourse;

public sealed class UpdateCourseCommandHandler(
    ICourseReadRepository courseReadRepository,
    ICourseWriteRepository courseWriteRepository,
    ICurrentUser currentUser) : IRequestHandler<UpdateCourseCommand, bool>
{
    public async Task<bool> Handle(UpdateCourseCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = currentUser.Id ?? throw new UnauthorizedAccessException("Kullanıcı kimliği bulunamadı.");

        var course = await courseReadRepository.GetByIdAsync(request.CourseId, cancellationToken);
        if (course is null)
        {
            throw new ResourceNotFoundException("Course", request.CourseId);
        }

        // Yetki kontrolü: CreatedBy veya Admin/OrganizationManager
        var isAdmin = currentUser.IsInRole(RoleNames.Administrator);
        var isOrgManager = currentUser.IsInRole(RoleNames.OrganizationManager);
        var isCreator = course.CreatedBy == currentUserId.Value;

        if (!isAdmin && !isOrgManager && !isCreator)
        {
            throw new UnauthorizedAccessException("Kursu güncellemek için yetkiniz bulunmamaktadır.");
        }

        course.UpdateDetails(request.Title, request.Description, request.ThumbnailUrl);

        courseWriteRepository.Update(course);
        
        return true;
    }
}