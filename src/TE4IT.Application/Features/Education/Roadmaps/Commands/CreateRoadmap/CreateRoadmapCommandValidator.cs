using FluentValidation;
using TE4IT.Domain.Enums.Education;

namespace TE4IT.Application.Features.Education.Roadmaps.Commands.CreateRoadmap;

/// <summary>
/// CreateRoadmapCommand için validation kuralları
/// </summary>
public sealed class CreateRoadmapCommandValidator : AbstractValidator<CreateRoadmapCommand>
{
    public CreateRoadmapCommandValidator()
    {
        RuleFor(x => x.CourseId)
            .NotEmpty()
            .WithMessage("Kurs ID'si zorunludur.");

        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Roadmap başlığı zorunludur.")
            .MinimumLength(3)
            .WithMessage("Roadmap başlığı en az 3 karakter olmalıdır.")
            .MaximumLength(200)
            .WithMessage("Roadmap başlığı en fazla 200 karakter olabilir.");

        RuleFor(x => x.EstimatedDurationMinutes)
            .GreaterThan(0)
            .WithMessage("Tahmini süre 0'dan büyük olmalıdır.");

        RuleFor(x => x.Steps)
            .NotEmpty()
            .WithMessage("En az bir adım gereklidir.");

        RuleForEach(x => x.Steps)
            .SetValidator(new StepDtoValidator());

        RuleFor(x => x.Steps)
            .Must(steps => steps.Select(s => s.Order).Distinct().Count() == steps.Count)
            .WithMessage("Adım sıraları benzersiz olmalıdır.")
            .Must(steps => steps.Select(s => s.Order).SequenceEqual(Enumerable.Range(1, steps.Count)))
            .WithMessage("Adım sıraları 1'den başlayarak ardışık olmalıdır.");
    }
}

public sealed class StepDtoValidator : AbstractValidator<StepDto>
{
    public StepDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Adım başlığı zorunludur.")
            .MinimumLength(3)
            .WithMessage("Adım başlığı en az 3 karakter olmalıdır.")
            .MaximumLength(200)
            .WithMessage("Adım başlığı en fazla 200 karakter olabilir.");

        RuleFor(x => x.Order)
            .GreaterThan(0)
            .WithMessage("Adım sırası 0'dan büyük olmalıdır.");

        RuleFor(x => x.EstimatedDurationMinutes)
            .GreaterThan(0)
            .WithMessage("Adım tahmini süresi 0'dan büyük olmalıdır.");

        RuleFor(x => x.Contents)
            .NotEmpty()
            .WithMessage("Adım içeriği en az bir içerik gerektirir.");

        RuleForEach(x => x.Contents)
            .SetValidator(new ContentDtoValidator());
    }
}

public sealed class ContentDtoValidator : AbstractValidator<ContentDto>
{
    public ContentDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("İçerik başlığı zorunludur.")
            .MinimumLength(3)
            .WithMessage("İçerik başlığı en az 3 karakter olmalıdır.")
            .MaximumLength(200)
            .WithMessage("İçerik başlığı en fazla 200 karakter olabilir.");

        RuleFor(x => x.Order)
            .GreaterThan(0)
            .WithMessage("İçerik sırası 0'dan büyük olmalıdır.");

        RuleFor(x => x.Type)
            .IsInEnum()
            .WithMessage("Geçerli bir içerik tipi seçiniz.");

        // Type'a göre Content veya LinkUrl zorunlu
        When(x => x.Type == ContentType.Text, () =>
        {
            RuleFor(x => x.Content)
                .NotEmpty()
                .WithMessage("Metin içeriği zorunludur.");
        });

        When(x => x.Type == ContentType.VideoLink || x.Type == ContentType.DocumentLink || x.Type == ContentType.ExternalLink, () =>
        {
            RuleFor(x => x.LinkUrl)
                .NotEmpty()
                .WithMessage("Link URL zorunludur.")
                .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _))
                .WithMessage("Geçerli bir URL formatı giriniz.");
        });
    }
}

