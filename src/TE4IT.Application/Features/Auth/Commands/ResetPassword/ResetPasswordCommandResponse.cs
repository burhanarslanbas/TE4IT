using MediatR;

namespace TE4IT.Application.Features.Auth.Commands.ResetPassword;

public sealed record ResetPasswordCommandResponse(bool Success, string Message);