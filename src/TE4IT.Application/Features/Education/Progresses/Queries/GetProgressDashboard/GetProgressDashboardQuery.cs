using MediatR;

namespace TE4IT.Application.Features.Education.Progresses.Queries.GetProgressDashboard;

public sealed record GetProgressDashboardQuery() : IRequest<ProgressDashboardResponse>;

