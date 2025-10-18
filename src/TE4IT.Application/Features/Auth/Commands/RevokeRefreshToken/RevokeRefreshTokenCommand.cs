using MediatR;

namespace TE4IT.Application.Features.Auth.Commands.RevokeRefreshToken;

/// <summary>
/// Refresh token'ı iptal etme komutu
/// </summary>
public sealed record RevokeRefreshTokenCommand(string RefreshToken) : IRequest<bool>;


