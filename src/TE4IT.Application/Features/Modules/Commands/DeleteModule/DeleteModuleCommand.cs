using MediatR;

namespace TE4IT.Application.Features.Modules.Commands.DeleteModule;

public sealed record DeleteModuleCommand(Guid ModuleId) : IRequest<bool>;

