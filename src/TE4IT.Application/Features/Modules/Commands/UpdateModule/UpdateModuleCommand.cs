using MediatR;

namespace TE4IT.Application.Features.Modules.Commands.UpdateModule;

public sealed record UpdateModuleCommand(Guid ModuleId, string Title, string? Description) : IRequest<bool>;

