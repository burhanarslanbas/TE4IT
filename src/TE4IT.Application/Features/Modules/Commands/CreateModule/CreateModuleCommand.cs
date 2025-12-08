using MediatR;

namespace TE4IT.Application.Features.Modules.Commands.CreateModule;

public sealed record CreateModuleCommand(
    Guid ProjectId,
    string Title,
    string? Description) : IRequest<CreateModuleCommandResponse>;

