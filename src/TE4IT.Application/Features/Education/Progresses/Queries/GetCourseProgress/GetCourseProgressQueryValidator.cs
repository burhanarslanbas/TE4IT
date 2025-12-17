using FluentValidation;

namespace TE4IT.Application.Features.Education.Progresses.Queries.GetCourseProgress;

/// <summary>
/// GetCourseProgressQuery için validation kuralları
/// </summary>
public sealed class GetCourseProgressQueryValidator : AbstractValidator<GetCourseProgressQuery>
{
    public GetCourseProgressQueryValidator()
    {
        RuleFor(x => x.CourseId)
            .NotEmpty()
            .WithMessage("Kurs ID'si zorunludur.");
    }
}

