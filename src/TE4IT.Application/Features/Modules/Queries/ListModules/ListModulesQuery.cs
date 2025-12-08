using MediatR;
using TE4IT.Application.Common.Pagination;
using TE4IT.Application.Features.Modules.Responses;

namespace TE4IT.Application.Features.Modules.Queries.ListModules;

public sealed record ListModulesQuery(
    Guid ProjectId,
    int Page = 1,
    int PageSize = 20,
    bool? IsActive = null,
    string? Search = null) : IRequest<PagedResult<ModuleListItemResponse>>;

