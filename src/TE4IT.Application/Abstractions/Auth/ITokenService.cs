namespace TE4IT.Application.Abstractions.Auth;

public interface ITokenService
{
    (string accessToken, DateTime expiresAt) CreateAccessToken(Guid userId, string userName, string email, IEnumerable<string> roles, IEnumerable<string> permissions, string? permissionsVersion = null);
    (string refreshToken, DateTime expiresAt) CreateRefreshToken(Guid userId);
}
