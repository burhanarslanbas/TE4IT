using MediatR;
using TE4IT.Application.Common.Pagination;
using TE4IT.Application.Features.Auth.Queries.Users.GetUserById;

namespace TE4IT.Application.Features.Auth.Queries.Users.GetAllUsers;

/// <summary>
/// Tüm kullanıcıları getirme sorgusu
/// </summary>
public sealed record GetAllUsersQuery(int Page = 1, int PageSize = 20) : IRequest<PagedResult<UserResponse>>;