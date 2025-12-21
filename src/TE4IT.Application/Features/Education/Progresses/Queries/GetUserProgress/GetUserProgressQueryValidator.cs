using FluentValidation;

namespace TE4IT.Application.Features.Education.Progresses.Queries.GetUserProgress;

/// <summary>
/// GetUserProgressQuery için validation kuralları
/// </summary>
public sealed class GetUserProgressQueryValidator : AbstractValidator<GetUserProgressQuery>
{
    public GetUserProgressQueryValidator()
    {
        // CourseId opsiyonel, validation gerekmez
    }
}

