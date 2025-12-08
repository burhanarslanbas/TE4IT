using FluentValidation;

namespace TE4IT.Application.Features.Modules.Commands.DeleteModule;

public sealed class DeleteModuleCommandValidator : AbstractValidator<DeleteModuleCommand>
{
    public DeleteModuleCommandValidator()
    {
        RuleFor(x => x.ModuleId).NotEmpty();
    }
}

