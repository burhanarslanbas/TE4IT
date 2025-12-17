using FluentValidation;

namespace TE4IT.Application.Features.Invitations.Queries.ListMyPendingInvitations;

public sealed class ListMyPendingInvitationsQueryValidator : AbstractValidator<ListMyPendingInvitationsQuery>
{
    public ListMyPendingInvitationsQueryValidator()
    {
        // Query has no parameters, validation is handled by authentication pipeline
    }
}
