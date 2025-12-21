using FluentValidation;

namespace TE4IT.Application.Features.Education.Courses.Queries.GetCourseById;

/// <summary>
/// GetCourseByIdQuery için validation kuralları
/// </summary>
public sealed class GetCourseByIdQueryValidator : AbstractValidator<GetCourseByIdQuery>
{
    public GetCourseByIdQueryValidator()
    {
        RuleFor(x => x.CourseId)
            .NotEmpty()
            .WithMessage("Kurs ID'si zorunludur.");
    }
}

