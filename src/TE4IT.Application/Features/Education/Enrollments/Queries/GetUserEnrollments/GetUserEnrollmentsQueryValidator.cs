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
            .Must(status => string.IsNullOrEmpty(status) || status == "active" || status == "completed" || status == "all")
            .WithMessage("Status 'active', 'completed' veya 'all' olmalıdır.");
    }
}

