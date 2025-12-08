using FluentValidation;
using TE4IT.Domain.Enums;

namespace TE4IT.Application.Features.TaskRelations.Commands.CreateTaskRelation;

public sealed class CreateTaskRelationCommandValidator : AbstractValidator<CreateTaskRelationCommand>
{
    public CreateTaskRelationCommandValidator()
    {
        RuleFor(x => x.SourceTaskId)
            .NotEmpty()
            .WithMessage("Kaynak görev ID'si zorunludur.");

        RuleFor(x => x.TargetTaskId)
            .NotEmpty()
            .WithMessage("Hedef görev ID'si zorunludur.")
            .NotEqual(x => x.SourceTaskId)
            .WithMessage("Kaynak ve hedef görev aynı olamaz.");

        RuleFor(x => x.RelationType)
            .IsInEnum()
            .WithMessage("Geçerli bir ilişki tipi seçilmelidir.");
    }
}

