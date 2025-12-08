using MediatR;
using TE4IT.Application.Common.Pagination;
using TE4IT.Application.Features.Tasks.Responses;
using TE4IT.Domain.Enums;

namespace TE4IT.Application.Features.Tasks.Queries.ListTasks;

public sealed record ListTasksQuery(
    Guid UseCaseId,
    int Page = 1,
    int PageSize = 20,
    TaskState? State = null,
    TaskType? Type = null,
    Guid? AssigneeId = null,
    DateTime? DueDateFrom = null,
    DateTime? DueDateTo = null,
    string? Search = null) : IRequest<PagedResult<TaskListItemResponse>>;

