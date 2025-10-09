using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using TE4IT.Application.Abstractions.Auth;
using TE4IT.Domain.ValueObjects;

namespace TE4IT.Infrastructure.Auth.Services;

public sealed class CurrentUserService(IHttpContextAccessor accessor) : ICurrentUser
{
    public UserId? Id
        => Guid.TryParse(accessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier), out var id)
            ? new UserId(id)
            : null;

    public bool IsAuthenticated => accessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

    public bool IsInRole(string role) => accessor.HttpContext?.User?.IsInRole(role) ?? false;

    public IEnumerable<string> Roles
        => accessor.HttpContext?.User?.Claims
               .Where(c => c.Type == ClaimTypes.Role)
               .Select(c => c.Value) ?? Array.Empty<string>();
}

