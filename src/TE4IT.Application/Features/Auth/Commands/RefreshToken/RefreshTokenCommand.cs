using MediatR;

namespace TE4IT.Application.Features.Auth.Commands.RefreshToken;

public sealed record RefreshTokenCommand(string RefreshToken) : IRequest<RefreshTokenCommandResponse>;

public sealed record RefreshTokenCommandResponse(string AccessToken, DateTime ExpiresAt, string RefreshToken, DateTime RefreshExpires);


