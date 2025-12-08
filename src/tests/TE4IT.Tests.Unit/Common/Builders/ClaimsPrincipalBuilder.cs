using System.Security.Claims;
using TE4IT.Domain.Constants;

namespace TE4IT.Tests.Unit.Common.Builders;

/// <summary>
/// Fluent API ile ClaimsPrincipal oluşturma builder'ı
/// </summary>
public class ClaimsPrincipalBuilder
{
    private readonly List<Claim> _claims = new();
    private bool _isAuthenticated = true;

    public ClaimsPrincipalBuilder WithUserId(Guid userId)
    {
        _claims.Add(new Claim(ClaimTypes.NameIdentifier, userId.ToString()));
        return this;
    }

    public ClaimsPrincipalBuilder WithRole(string role)
    {
        _claims.Add(new Claim(ClaimTypes.Role, role));
        return this;
    }

    public ClaimsPrincipalBuilder WithPermission(string permission)
    {
        _claims.Add(new Claim("permission", permission));
        return this;
    }

    public ClaimsPrincipalBuilder WithClaim(string type, string value)
    {
        _claims.Add(new Claim(type, value));
        return this;
    }

    public ClaimsPrincipalBuilder WithEmail(string email)
    {
        _claims.Add(new Claim(ClaimTypes.Email, email));
        return this;
    }

    public ClaimsPrincipalBuilder WithUserName(string userName)
    {
        _claims.Add(new Claim(ClaimTypes.Name, userName));
        return this;
    }

    public ClaimsPrincipalBuilder AsAuthenticated(bool isAuthenticated = true)
    {
        _isAuthenticated = isAuthenticated;
        return this;
    }

    public ClaimsPrincipal Build()
    {
        var identity = new ClaimsIdentity(_claims, "Test", ClaimTypes.Name, ClaimTypes.Role);
        return new ClaimsPrincipal(identity);
    }

    public static ClaimsPrincipalBuilder Create() => new();
}
