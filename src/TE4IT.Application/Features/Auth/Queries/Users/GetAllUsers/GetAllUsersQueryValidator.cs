using FluentValidation;

namespace TE4IT.Application.Features.Auth.Queries.Users.GetAllUsers;

/// <summary>
/// GetAllUsersQuery validator
/// </summary>
public sealed class GetAllUsersQueryValidator : AbstractValidator<GetAllUsersQuery>
{
    public GetAllUsersQueryValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0)
            .WithMessage("Sayfa numarası 1'den büyük olmalıdır.");
        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .WithMessage("Sayfa boyutu 0'dan büyük olmalıdır.")
            .LessThanOrEqualTo(100)
            .WithMessage("Sayfa boyutu en fazla 100 olabilir.");
    }
}