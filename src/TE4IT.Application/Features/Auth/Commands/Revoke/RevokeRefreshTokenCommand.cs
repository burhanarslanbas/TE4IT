using MediatR;

namespace TE4IT.Application.Features.Auth.Commands.Revoke;

public sealed record RevokeRefreshTokenCommand(string RefreshToken, string Reason, string IpAddress) : IRequest<bool>;


