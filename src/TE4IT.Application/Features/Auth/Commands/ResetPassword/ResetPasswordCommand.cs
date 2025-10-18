using MediatR;

namespace TE4IT.Application.Features.Auth.Commands.ResetPassword;

public sealed record ResetPasswordCommand(string Email, string Token, string NewPassword) : IRequest<ResetPasswordCommandResponse>;