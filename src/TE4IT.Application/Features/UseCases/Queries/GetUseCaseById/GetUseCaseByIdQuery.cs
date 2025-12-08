using MediatR;
using TE4IT.Application.Features.UseCases.Responses;

namespace TE4IT.Application.Features.UseCases.Queries.GetUseCaseById;

public sealed record GetUseCaseByIdQuery(Guid UseCaseId) : IRequest<UseCaseResponse>;

