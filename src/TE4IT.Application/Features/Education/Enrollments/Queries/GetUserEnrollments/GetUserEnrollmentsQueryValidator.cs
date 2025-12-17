using FluentValidation;

namespace TE4IT.Application.Features.Education.Enrollments.Queries.GetUserEnrollments;

/// <summary>
/// GetUserEnrollmentsQuery için validation kuralları
/// </summary>
public sealed class GetUserEnrollmentsQueryValidator : AbstractValidator<GetUserEnrollmentsQuery>
{
    public GetUserEnrollmentsQueryValidator()
    {
        RuleFor(x => x.Status)
            .Must(status => status == "active" || status == "completed" || status == "all")
            .When(x => !string.IsNullOrEmpty(x.Status))
            .WithMessage("Status 'active', 'completed' veya 'all' olmalıdır.");
    }
}

