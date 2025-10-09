namespace TE4IT.Application.Features.Auth.Queries.Users.GetUserById;

/// <summary>
/// Kullanıcı bilgileri DTO
/// </summary>
public sealed class UserResponse
{
    public Guid Id { get; init; }
    public string UserName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public bool EmailConfirmed { get; init; }
    public IEnumerable<string> Roles { get; init; } = Array.Empty<string>();
}

