using MediatR;

namespace TE4IT.Application.Features.UseCases.Commands.UpdateUseCase;

public sealed record UpdateUseCaseCommand(
    Guid UseCaseId,
    string Title,
    string? Description = null,
    string? ImportantNotes = null) : IRequest<bool>;

