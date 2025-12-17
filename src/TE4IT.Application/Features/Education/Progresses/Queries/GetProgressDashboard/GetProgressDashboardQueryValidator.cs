using FluentValidation;

namespace TE4IT.Application.Features.Education.Progresses.Queries.GetProgressDashboard;

/// <summary>
/// GetProgressDashboardQuery için validation kuralları (boş query, validation gerekmez)
/// </summary>
public sealed class GetProgressDashboardQueryValidator : AbstractValidator<GetProgressDashboardQuery>
{
    public GetProgressDashboardQueryValidator()
    {
        // Boş query, validation gerekmez
    }
}

