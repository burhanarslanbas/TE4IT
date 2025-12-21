using MediatR;
using TE4IT.Application.Abstractions.Auth;
using TE4IT.Application.Abstractions.Persistence.Repositories.Education.Courses;
using TE4IT.Domain.Constants;
using TE4IT.Domain.Entities.Education;
using TE4IT.Domain.Enums.Education;
using TE4IT.Domain.Exceptions.Common;

namespace TE4IT.Application.Features.Education.Roadmaps.Commands.CreateRoadmap;

public sealed class CreateRoadmapCommandHandler(
    ICourseReadRepository courseReadRepository,
    ICourseWriteRepository courseWriteRepository,
    ICurrentUser currentUser) : IRequestHandler<CreateRoadmapCommand, CreateRoadmapCommandResponse>
{
    public async Task<CreateRoadmapCommandResponse> Handle(CreateRoadmapCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = currentUser.Id ?? throw new UnauthorizedAccessException("Kullanıcı kimliği bulunamadı.");

        var course = await courseReadRepository.GetByIdAsync(request.CourseId, cancellationToken);
        if (course is null)
        {
            throw new ResourceNotFoundException("Course", request.CourseId);
        }

        // Course'un zaten roadmap'i varsa hata
        if (course.Roadmap is not null)
        {
            throw new BusinessRuleViolationException("Kursun zaten bir roadmap'i bulunmaktadır.");
        }

        // Yetki kontrolü: CreatedBy veya Admin/OrganizationManager
        var isAdmin = currentUser.IsInRole(RoleNames.Administrator);
        var isOrgManager = currentUser.IsInRole(RoleNames.OrganizationManager);
        var isCreator = course.CreatedBy == currentUserId.Value;

        if (!isAdmin && !isOrgManager && !isCreator)
        {
            throw new UnauthorizedAccessException("Roadmap oluşturmak için yetkiniz bulunmamaktadır.");
        }

        // Roadmap ve Steps oluştur
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

        return new CreateRoadmapCommandResponse { RoadmapId = roadmap.Id };
    }
}

