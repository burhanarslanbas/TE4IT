using FluentValidation;
using TE4IT.Domain.Enums;
using TE4IT.Domain.Services;

namespace TE4IT.Application.Features.Projects.Commands.AddProjectMember;

public sealed class AddProjectMemberCommandValidator : AbstractValidator<AddProjectMemberCommand>
{
    public AddProjectMemberCommandValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotEmpty()
            .WithMessage("Proje ID'si zorunludur.");

        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("Kullanıcı ID'si zorunludur.");

        RuleFor(x => x.Role)
            .IsInEnum()
            .WithMessage("Geçerli bir rol seçilmelidir.")
            .Must(role => role == ProjectRole.Member || role == ProjectRole.Viewer)
            .WithMessage("Sadece Member veya Viewer rolü atanabilir. Owner rolü sadece proje oluşturulurken atanır.");
    }
}

