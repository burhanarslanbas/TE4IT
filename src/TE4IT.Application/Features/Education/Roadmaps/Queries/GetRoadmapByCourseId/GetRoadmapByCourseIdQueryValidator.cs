using FluentValidation;

namespace TE4IT.Application.Features.Education.Roadmaps.Queries.GetRoadmapByCourseId;

/// <summary>
/// GetRoadmapByCourseIdQuery için validation kuralları
/// </summary>
public sealed class GetRoadmapByCourseIdQueryValidator : AbstractValidator<GetRoadmapByCourseIdQuery>
{
    public GetRoadmapByCourseIdQueryValidator()
    {
        RuleFor(x => x.CourseId)
            .NotEmpty()
            .WithMessage("Kurs ID'si zorunludur.");
    }
}

