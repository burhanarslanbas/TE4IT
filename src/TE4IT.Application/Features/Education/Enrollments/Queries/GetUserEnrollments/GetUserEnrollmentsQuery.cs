using MediatR;

namespace TE4IT.Application.Features.Education.Enrollments.Queries.GetUserEnrollments;

public sealed record GetUserEnrollmentsQuery(string? Status = "all") : IRequest<IReadOnlyList<EnrollmentListItemResponse>>;

