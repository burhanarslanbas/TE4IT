using MediatR;
using TE4IT.Application.Abstractions.Auth;

namespace TE4IT.Application.Features.Auth.Commands.Revoke;

public sealed class RevokeRefreshTokenCommandHandler(IRefreshTokenService refreshTokens) : IRequestHandler<RevokeRefreshTokenCommand, bool>
{
    public async Task<bool> Handle(RevokeRefreshTokenCommand request, CancellationToken cancellationToken)
    {
        return await refreshTokens.RevokeAsync(request.RefreshToken, request.IpAddress, request.Reason, cancellationToken);
    }
}


