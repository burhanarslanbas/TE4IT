using MediatR;

namespace TE4IT.Application.Features.Auth.Queries.Users.GetUserRoles;

/// <summary>
/// Kullanıcının rollerini getirme sorgusu
/// </summary>
public sealed record GetUserRolesQuery(Guid UserId) : IRequest<IEnumerable<string>>;

