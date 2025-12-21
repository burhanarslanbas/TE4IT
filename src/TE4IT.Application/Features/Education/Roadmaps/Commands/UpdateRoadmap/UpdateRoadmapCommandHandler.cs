using MediatR;
using TE4IT.Application.Abstractions.Auth;
using TE4IT.Application.Abstractions.Persistence.Repositories.Education.Courses;
using TE4IT.Domain.Constants;
using TE4IT.Domain.Entities.Education;
using TE4IT.Domain.Enums.Education;
using TE4IT.Domain.Exceptions.Common;

namespace TE4IT.Application.Features.Education.Roadmaps.Commands.UpdateRoadmap;

public sealed class UpdateRoadmapCommandHandler(
    ICourseReadRepository courseReadRepository,
    ICourseWriteRepository courseWriteRepository,
    ICurrentUser currentUser) : IRequestHandler<UpdateRoadmapCommand, bool>
{
    public async Task<bool> Handle(UpdateRoadmapCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = currentUser.Id ?? throw new UnauthorizedAccessException("Kullanıcı kimliği bulunamadı.");

        var course = await courseReadRepository.GetByIdAsync(request.CourseId, cancellationToken);
        if (course is null)
        {
            throw new ResourceNotFoundException("Course", request.CourseId);
        }

        if (course.Roadmap is null)
        {
            throw new ResourceNotFoundException("Roadmap", request.CourseId);
        }

        // Yetki kontrolü
        var isAdmin = currentUser.IsInRole(RoleNames.Administrator);
        var isOrgManager = currentUser.IsInRole(RoleNames.OrganizationManager);
        var isCreator = course.CreatedBy == currentUserId.Value;

        if (!isAdmin && !isOrgManager && !isCreator)
        {
            throw new UnauthorizedAccessException("Roadmap güncellemek için yetkiniz bulunmamaktadır.");
        }

        // Roadmap ve Steps oluştur (tüm steps ve contents replace edilir)
        var roadmap = new CourseRoadmap(request.Title, request.EstimatedDurationMinutes, request.Description);

        var steps = request.Steps.Select(stepDto => new RoadmapStep(
            stepDto.Title,
            stepDto.Order,
            stepDto.IsRequired,
            stepDto.EstimatedDurationMinutes,
            stepDto.Description)).ToList();

        // Contents oluştur
        for (int i = 0; i < request.Steps.Count; i++)
        {
            var stepDto = request.Steps[i];
            var step = steps[i];

            var contents = stepDto.Contents.Select(contentDto => new CourseContent(
                contentDto.Type,
                contentDto.Title,
                contentDto.Order,
                contentDto.IsRequired,
                contentDto.Content,
                contentDto.LinkUrl,
                embedUrl: null,
                platform: null,
                description: contentDto.Description)).ToList();

            step.SetContents(contents);
        }

        roadmap.SetSteps(steps);
        course.SetRoadmap(roadmap);

        courseWriteRepository.Update(course);

        return true;
    }
}

