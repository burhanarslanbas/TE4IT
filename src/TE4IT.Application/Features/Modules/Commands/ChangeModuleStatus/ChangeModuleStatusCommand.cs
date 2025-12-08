using MediatR;

namespace TE4IT.Application.Features.Modules.Commands.ChangeModuleStatus;

public sealed record ChangeModuleStatusCommand(Guid ModuleId, bool IsActive) : IRequest<bool>;

