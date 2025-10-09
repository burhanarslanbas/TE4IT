using FluentValidation;

namespace TE4IT.Application.Features.Auth.Commands.Revoke;

public sealed class RevokeRefreshTokenCommandValidator : AbstractValidator<RevokeRefreshTokenCommand>
{
    public RevokeRefreshTokenCommandValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty()
            .WithMessage("Refresh token zorunludur.");

        RuleFor(x => x.IpAddress)
            .NotEmpty()
            .WithMessage("IP adresi zorunludur.");

        RuleFor(x => x.Reason)
            .MaximumLength(500)
            .When(x => !string.IsNullOrEmpty(x.Reason))
            .WithMessage("İptal nedeni en fazla 500 karakter olabilir.");
    }
}

