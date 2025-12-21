using FluentValidation;

namespace TE4IT.Application.Features.Education.Progresses.Commands.UpdateProgress;

/// <summary>
/// UpdateProgressCommand için validation kuralları
/// </summary>
public sealed class UpdateProgressCommandValidator : AbstractValidator<UpdateProgressCommand>
{
    public UpdateProgressCommandValidator()
    {
        RuleFor(x => x.ContentId)
            .NotEmpty()
            .WithMessage("İçerik ID'si zorunludur.");

        RuleFor(x => x.CourseId)
            .NotEmpty()
            .WithMessage("Kurs ID'si zorunludur.");

        RuleFor(x => x.TimeSpentMinutes)
            .GreaterThan(0)
            .When(x => x.TimeSpentMinutes.HasValue)
            .WithMessage("Harcanan süre 0'dan büyük olmalıdır.");

        RuleFor(x => x.WatchedPercentage)
            .InclusiveBetween(0, 100)
            .When(x => x.WatchedPercentage.HasValue)
            .WithMessage("İzlenme yüzdesi 0-100 arasında olmalıdır.");
    }
}

