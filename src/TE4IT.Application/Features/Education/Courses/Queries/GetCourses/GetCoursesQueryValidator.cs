using FluentValidation;

namespace TE4IT.Application.Features.Education.Courses.Queries.GetCourses;

/// <summary>
/// GetCoursesQuery için validation kuralları
/// </summary>
public sealed class GetCoursesQueryValidator : AbstractValidator<GetCoursesQuery>
{
    public GetCoursesQueryValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0)
            .WithMessage("Sayfa numarası 0'dan büyük olmalıdır.");

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .LessThanOrEqualTo(100)
            .WithMessage("Sayfa boyutu 1 ile 100 arasında olmalıdır.");
    }
}

