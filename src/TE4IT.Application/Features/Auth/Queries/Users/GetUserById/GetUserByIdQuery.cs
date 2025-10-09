using MediatR;

namespace TE4IT.Application.Features.Auth.Queries.Users.GetUserById;

/// <summary>
/// Kullanıcı ID'sine göre getirme sorgusu
/// </summary>
public sealed record GetUserByIdQuery(Guid UserId) : IRequest<UserResponse?>;

