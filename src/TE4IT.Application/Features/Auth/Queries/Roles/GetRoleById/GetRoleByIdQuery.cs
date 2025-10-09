using MediatR;
using TE4IT.Application.Features.Auth.Queries.Roles.GetAllRoles;

namespace TE4IT.Application.Features.Auth.Queries.Roles.GetRoleById;

/// <summary>
/// Rol ID'sine göre getirme sorgusu
/// </summary>
public sealed record GetRoleByIdQuery(Guid RoleId) : IRequest<RoleResponse?>;

