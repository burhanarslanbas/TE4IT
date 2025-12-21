using FluentValidation;

namespace TE4IT.Application.Features.Education.Enrollments.Commands.EnrollInCourse;

/// <summary>
/// EnrollInCourseCommand için validation kuralları
/// </summary>
public sealed class EnrollInCourseCommandValidator : AbstractValidator<EnrollInCourseCommand>
{
    public EnrollInCourseCommandValidator()
    {
        RuleFor(x => x.CourseId)
            .NotEmpty()
            .WithMessage("Kurs ID'si zorunludur.");
    }
}

