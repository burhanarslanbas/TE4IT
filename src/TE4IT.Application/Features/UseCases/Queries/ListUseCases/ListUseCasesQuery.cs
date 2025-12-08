using MediatR;
using TE4IT.Application.Common.Pagination;
using TE4IT.Application.Features.UseCases.Responses;

namespace TE4IT.Application.Features.UseCases.Queries.ListUseCases;

public sealed record ListUseCasesQuery(
    Guid ModuleId,
    int Page = 1,
    int PageSize = 20,
    bool? IsActive = null,
    string? Search = null) : IRequest<PagedResult<UseCaseListItemResponse>>;

