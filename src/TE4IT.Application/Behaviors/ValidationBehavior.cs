using FluentValidation;
using MediatR;

namespace TE4IT.Application.Behaviors;

/// <summary>
/// MediatR pipeline'da validation yapan behavior
/// Her request için otomatik validation çalıştırır
/// </summary>
public sealed class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        // Validator yoksa direkt devam et
        if (!validators.Any())
        {
            return await next();
        }

        // Validation context oluştur
        var context = new ValidationContext<TRequest>(request);

        // Tüm validator'ları paralel çalıştır
        var validationResults = await Task.WhenAll(
            validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        // Validation hatalarını topla
        var failures = validationResults
            .Where(r => r.Errors.Any())
            .SelectMany(r => r.Errors)
            .ToList();

        // Hata varsa exception fırlat (GlobalExceptionMiddleware yakalar)
        if (failures.Any())
        {
            throw new ValidationException(failures);
        }

        // Validation başarılı, devam et
        return await next();
    }
}

