namespace TE4IT.Application.Abstractions.Auth;

public sealed record UserInfo(Guid Id, string UserName, string Email, IEnumerable<string> Roles, string? PermissionsVersion);

public interface IUserInfoService
{
    Task<UserInfo?> GetUserInfoAsync(Guid userId, CancellationToken ct);
}


