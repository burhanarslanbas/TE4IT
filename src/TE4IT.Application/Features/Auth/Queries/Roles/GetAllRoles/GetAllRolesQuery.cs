using MediatR;

namespace TE4IT.Application.Features.Auth.Queries.Roles.GetAllRoles;

/// <summary>
/// Tüm rolleri getirme sorgusu
/// </summary>
public sealed record GetAllRolesQuery : IRequest<IEnumerable<RoleResponse>>;

