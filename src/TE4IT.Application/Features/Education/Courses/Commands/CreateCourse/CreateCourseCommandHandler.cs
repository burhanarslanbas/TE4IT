using MediatR;
using TE4IT.Application.Abstractions.Auth;
using TE4IT.Application.Abstractions.Persistence.Repositories.Education.Courses;
using TE4IT.Domain.Constants;
using TE4IT.Domain.Entities.Education;
using TE4IT.Domain.Events;

namespace TE4IT.Application.Features.Education.Courses.Commands.CreateCourse;

public sealed class CreateCourseCommandHandler(
    ICourseWriteRepository courseRepository,
    ICurrentUser currentUser) : IRequestHandler<CreateCourseCommand, CreateCourseCommandResponse>
{
    public async Task<CreateCourseCommandResponse> Handle(CreateCourseCommand request, CancellationToken cancellationToken)
    {
        var creatorId = currentUser.Id ?? throw new UnauthorizedAccessException("Kullanıcı kimliği bulunamadı.");

        // Yetki kontrolü: Admin veya OrganizationManager olmalı
        var isAdmin = currentUser.IsInRole(RoleNames.Administrator);
        var isOrgManager = currentUser.IsInRole(RoleNames.OrganizationManager);
        
        if (!isAdmin && !isOrgManager)
        {
            throw new UnauthorizedAccessException("Kurs oluşturmak için Admin veya Kurum Müdürü yetkisi gereklidir.");
        }

        var course = new Course(request.Title, request.Description, creatorId.Value, request.ThumbnailUrl);
        // CourseCreatedEvent constructor'da otomatik fırlatılıyor

        await courseRepository.AddAsync(course, cancellationToken);

        return new CreateCourseCommandResponse { Id = course.Id };
    }
}

