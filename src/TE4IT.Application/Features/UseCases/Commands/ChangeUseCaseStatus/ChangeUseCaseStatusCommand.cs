using MediatR;

namespace TE4IT.Application.Features.UseCases.Commands.ChangeUseCaseStatus;

public sealed record ChangeUseCaseStatusCommand(Guid UseCaseId, bool IsActive) : IRequest<bool>;

