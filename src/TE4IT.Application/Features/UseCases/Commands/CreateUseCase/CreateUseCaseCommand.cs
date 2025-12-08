using MediatR;

namespace TE4IT.Application.Features.UseCases.Commands.CreateUseCase;

public sealed record CreateUseCaseCommand(
    Guid ModuleId,
    string Title,
    string? Description = null,
    string? ImportantNotes = null) : IRequest<CreateUseCaseCommandResponse>;

