using MediatR;

namespace TE4IT.Application.Features.Auth.Commands.Register;

public sealed record RegisterCommand(string UserName, string Email, string Password) : IRequest<RegisterCommandResponse?>;

public sealed record RegisterCommandResponse(
    Guid UserId,
    string UserName,
    string Email,
    string AccessToken,
    DateTime ExpiresAt,
    string RefreshToken,
    DateTime RefreshExpires
);


