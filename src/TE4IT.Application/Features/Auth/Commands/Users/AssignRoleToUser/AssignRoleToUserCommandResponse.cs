namespace TE4IT.Application.Features.Auth.Commands.Users.AssignRoleToUser;

/// <summary>
/// Rol atama sonucu
/// </summary>
public sealed class AssignRoleToUserCommandResponse
{
    public Guid UserId { get; init; }
    public string RoleName { get; init; } = string.Empty;
    public bool Success { get; init; }
    public string Message { get; init; } = string.Empty;
}

