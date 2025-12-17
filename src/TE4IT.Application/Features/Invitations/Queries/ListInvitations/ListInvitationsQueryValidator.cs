using FluentValidation;

namespace TE4IT.Application.Features.Invitations.Queries.ListInvitations;

public sealed class ListInvitationsQueryValidator : AbstractValidator<ListInvitationsQuery>
{
    public ListInvitationsQueryValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotEmpty()
            .WithMessage("Project ID is required.");

        RuleFor(x => x.Status)
            .Must(status => string.IsNullOrWhiteSpace(status) || 
                status.ToLowerInvariant() == "pending" ||
                status.ToLowerInvariant() == "accepted" ||
                status.ToLowerInvariant() == "cancelled" ||
                status.ToLowerInvariant() == "expired")
            .WithMessage("Status must be one of: pending, accepted, cancelled, expired.");
    }
}
