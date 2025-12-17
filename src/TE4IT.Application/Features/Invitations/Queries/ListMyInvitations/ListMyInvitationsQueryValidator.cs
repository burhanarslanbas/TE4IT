using FluentValidation;

namespace TE4IT.Application.Features.Invitations.Queries.ListMyInvitations;

public sealed class ListMyInvitationsQueryValidator : AbstractValidator<ListMyInvitationsQuery>
{
    public ListMyInvitationsQueryValidator()
    {
        RuleFor(x => x.Status)
            .Must(status => string.IsNullOrWhiteSpace(status) ||
                status.ToLowerInvariant() == "pending" ||
                status.ToLowerInvariant() == "accepted" ||
                status.ToLowerInvariant() == "cancelled" ||
                status.ToLowerInvariant() == "expired")
            .WithMessage("Status must be one of: pending, accepted, cancelled, expired, or null (all).");
    }
}
