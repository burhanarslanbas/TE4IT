using MediatR;

namespace TE4IT.Application.Features.Auth.Commands.Login;

public sealed record LoginCommand(string Email, string Password) : IRequest<LoginCommandResponse?>;

public sealed record LoginCommandResponse(
    Guid UserId,
    string Email,
    string AccessToken,
    DateTime ExpiresAt,
    string RefreshToken,
    DateTime RefreshExpires
);


