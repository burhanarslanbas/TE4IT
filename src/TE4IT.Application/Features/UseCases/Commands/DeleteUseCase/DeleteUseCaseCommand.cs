using MediatR;

namespace TE4IT.Application.Features.UseCases.Commands.DeleteUseCase;

public sealed record DeleteUseCaseCommand(Guid UseCaseId) : IRequest<bool>;

