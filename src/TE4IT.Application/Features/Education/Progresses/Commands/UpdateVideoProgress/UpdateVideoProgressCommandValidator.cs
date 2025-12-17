using FluentValidation;

namespace TE4IT.Application.Features.Education.Progresses.Commands.UpdateVideoProgress;

/// <summary>
/// UpdateVideoProgressCommand için validation kuralları
/// </summary>
public sealed class UpdateVideoProgressCommandValidator : AbstractValidator<UpdateVideoProgressCommand>
{
    public UpdateVideoProgressCommandValidator()
    {
        RuleFor(x => x.ContentId)
            .NotEmpty()
            .WithMessage("İçerik ID'si zorunludur.");

        RuleFor(x => x.CourseId)
            .NotEmpty()
            .WithMessage("Kurs ID'si zorunludur.");

        RuleFor(x => x.WatchedPercentage)
            .InclusiveBetween(0, 100)
            .WithMessage("İzlenme yüzdesi 0-100 arasında olmalıdır.");

        RuleFor(x => x.TimeSpentSeconds)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Harcanan süre 0 veya daha büyük olmalıdır.");
    }
}

