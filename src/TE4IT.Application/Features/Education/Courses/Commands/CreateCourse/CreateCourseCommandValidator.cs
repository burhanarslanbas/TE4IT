using FluentValidation;

namespace TE4IT.Application.Features.Education.Courses.Commands.CreateCourse;

/// <summary>
/// CreateCourseCommand için validation kuralları
/// </summary>
public sealed class CreateCourseCommandValidator : AbstractValidator<CreateCourseCommand>
{
    public CreateCourseCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Kurs başlığı zorunludur.")
            .MinimumLength(3)
            .WithMessage("Kurs başlığı en az 3 karakter olmalıdır.")
            .MaximumLength(200)
            .WithMessage("Kurs başlığı en fazla 200 karakter olabilir.");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Kurs açıklaması zorunludur.")
            .MaximumLength(2000)
            .WithMessage("Kurs açıklaması en fazla 2000 karakter olabilir.");

        RuleFor(x => x.ThumbnailUrl)
            .Must(uri => string.IsNullOrEmpty(uri) || Uri.TryCreate(uri, UriKind.Absolute, out _))
            .When(x => !string.IsNullOrEmpty(x.ThumbnailUrl))
            .WithMessage("Geçerli bir URL formatı giriniz.");
    }
}

