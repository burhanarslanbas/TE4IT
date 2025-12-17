using MediatR;
using TE4IT.Domain.Enums;

namespace TE4IT.Application.Features.Tasks.Commands.ChangeTaskState;

public sealed record ChangeTaskStateCommand(Guid TaskId, TaskState NewState) : IRequest<bool>;

