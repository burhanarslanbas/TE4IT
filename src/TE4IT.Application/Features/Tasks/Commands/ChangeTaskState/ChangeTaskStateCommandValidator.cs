using FluentValidation;
using TE4IT.Domain.Enums;

namespace TE4IT.Application.Features.Tasks.Commands.ChangeTaskState;

public sealed class ChangeTaskStateCommandValidator : AbstractValidator<ChangeTaskStateCommand>
{
    public ChangeTaskStateCommandValidator()
    {
        RuleFor(x => x.TaskId).NotEmpty();
        RuleFor(x => x.NewState)
            .IsInEnum()
            .WithMessage("Geçerli bir görev durumu seçilmelidir.");
    }
}

