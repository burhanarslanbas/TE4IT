using FluentValidation;

namespace TE4IT.Application.Features.Modules.Commands.ChangeModuleStatus;

public sealed class ChangeModuleStatusCommandValidator : AbstractValidator<ChangeModuleStatusCommand>
{
    public ChangeModuleStatusCommandValidator()
    {
        RuleFor(x => x.ModuleId).NotEmpty();
    }
}

