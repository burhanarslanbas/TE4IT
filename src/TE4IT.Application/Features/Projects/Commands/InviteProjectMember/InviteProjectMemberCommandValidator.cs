using FluentValidation;
using TE4IT.Domain.Enums;

namespace TE4IT.Application.Features.Projects.Commands.InviteProjectMember;

public sealed class InviteProjectMemberCommandValidator : AbstractValidator<InviteProjectMemberCommand>
{
    public InviteProjectMemberCommandValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotEmpty()
            .WithMessage("Proje ID'si zorunludur.");

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email adresi zorunludur.")
            .EmailAddress()
            .WithMessage("Geçerli bir email adresi giriniz.")
            .MaximumLength(256)
            .WithMessage("Email adresi en fazla 256 karakter olabilir.");

        RuleFor(x => x.Role)
            .IsInEnum()
            .WithMessage("Geçerli bir rol seçilmelidir.")
            .Must(role => role == ProjectRole.Member || role == ProjectRole.Viewer)
            .WithMessage("Sadece Member veya Viewer rolü atanabilir. Owner rolü sadece proje oluşturulurken atanır.");
    }
}

