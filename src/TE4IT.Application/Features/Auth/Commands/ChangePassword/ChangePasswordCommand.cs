using MediatR;

namespace TE4IT.Application.Features.Auth.Commands.ChangePassword;

public sealed record ChangePasswordCommand(
    string CurrentPassword, 
    string NewPassword) : IRequest<ChangePasswordCommandResponse>;
