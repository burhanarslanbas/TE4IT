using MediatR;

namespace TE4IT.Application.Features.Auth.Commands.Register;

public sealed record RegisterCommand(string Email, string Password) : IRequest<RegisterCommandResponse?>;

public sealed record RegisterCommandResponse(
    Guid UserId,
    string Email,
    string AccessToken,
    DateTime ExpiresAt,
    string RefreshToken,
    DateTime RefreshExpires
);


