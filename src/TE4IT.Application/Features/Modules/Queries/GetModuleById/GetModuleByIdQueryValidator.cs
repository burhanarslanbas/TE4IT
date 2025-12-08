using FluentValidation;

namespace TE4IT.Application.Features.Modules.Queries.GetModuleById;

public sealed class GetModuleByIdQueryValidator : AbstractValidator<GetModuleByIdQuery>
{
    public GetModuleByIdQueryValidator()
    {
        RuleFor(x => x.ModuleId).NotEmpty();
    }
}

