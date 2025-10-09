
using TE4IT.Domain.ValueObjects;

namespace TE4IT.Application.Abstractions.Auth;

public interface ICurrentUser
{
    UserId? Id { get; }
    bool IsAuthenticated { get; }
    bool IsInRole(string role);
    IEnumerable<string> Roles { get; }
}
