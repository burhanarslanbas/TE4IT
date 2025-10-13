using FluentValidation;

namespace TE4IT.Application.Features.Auth.Commands.Revoke;

/// <summary>
/// Refresh token iptal etme komut validatörü
/// </summary>
public sealed class RevokeRefreshTokenCommandValidator : AbstractValidator<RevokeRefreshTokenCommand>
{
    public RevokeRefreshTokenCommandValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty()
            .WithMessage("Refresh token zorunludur.")
            .MaximumLength(1000)
            .WithMessage("Refresh token en fazla 1000 karakter olabilir.");
    }
}

