using FluentValidation;

namespace TE4IT.Application.Features.Projects.Queries.GetProjectById;

public sealed class GetProjectByIdQueryValidator : AbstractValidator<GetProjectByIdQuery>
{
    public GetProjectByIdQueryValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotEmpty()
            .WithMessage("Proje ID'si zorunludur.");
    }
}

