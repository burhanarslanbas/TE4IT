using MediatR;
using TE4IT.Application.Features.Modules.Responses;

namespace TE4IT.Application.Features.Modules.Queries.GetModuleById;

public sealed record GetModuleByIdQuery(Guid ModuleId) : IRequest<ModuleResponse>;

