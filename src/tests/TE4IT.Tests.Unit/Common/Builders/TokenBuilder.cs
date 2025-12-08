using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using TE4IT.Domain.Constants;

namespace TE4IT.Tests.Unit.Common.Builders;

/// <summary>
/// Fluent API ile JWT token oluşturma builder'ı
/// </summary>
public class TokenBuilder
{
    private Guid _userId = Guid.NewGuid();
    private string _userName = "testuser";
    private string _email = "test@example.com";
    private List<string> _roles = new();
    private List<string> _permissions = new();
    private DateTime? _expiration;
    private string? _permissionsVersion;
    private string _issuer = "https://test-issuer.com";
    private string _audience = "https://test-audience.com";
    private SymmetricSecurityKey? _signingKey;

    public TokenBuilder WithUserId(Guid userId)
    {
        _userId = userId;
        return this;
    }

    public TokenBuilder WithRoles(IEnumerable<string> roles)
    {
        _roles = roles.ToList();
        return this;
    }

    public TokenBuilder WithRole(string role)
    {
        _roles.Add(role);
        return this;
    }

    public TokenBuilder WithPermissions(IEnumerable<string> permissions)
    {
        _permissions = permissions.ToList();
        return this;
    }

    public TokenBuilder WithPermission(string permission)
    {
        _permissions.Add(permission);
        return this;
    }

    public TokenBuilder WithExpiration(DateTime expiration)
    {
        _expiration = expiration;
        return this;
    }

    public TokenBuilder WithPermissionsVersion(string version)
    {
        _permissionsVersion = version;
        return this;
    }

    public TokenBuilder WithIssuer(string issuer)
    {
        _issuer = issuer;
        return this;
    }

    public TokenBuilder WithAudience(string audience)
    {
        _audience = audience;
        return this;
    }

    public TokenBuilder WithSigningKey(SymmetricSecurityKey key)
    {
        _signingKey = key;
        return this;
    }

    public JwtSecurityToken Build()
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, _userId.ToString()),
            new(JwtRegisteredClaimNames.Sub, _userId.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.UniqueName, _userName),
            new(JwtRegisteredClaimNames.Email, _email)
        };

        claims.AddRange(_roles.Select(r => new Claim(ClaimTypes.Role, r)));
        claims.AddRange(_permissions.Select(p => new Claim("permission", p)));

        if (!string.IsNullOrWhiteSpace(_permissionsVersion))
        {
            claims.Add(new Claim("permissions_version", _permissionsVersion));
        }

        var key = _signingKey ?? new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes("test-signing-key-that-is-at-least-32-characters-long"));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = _expiration ?? DateTime.UtcNow.AddMinutes(15);

        return new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: expires,
            signingCredentials: creds
        );
    }

    public string BuildTokenString()
    {
        var token = Build();
        var handler = new JwtSecurityTokenHandler();
        return handler.WriteToken(token);
    }

    public static TokenBuilder Create() => new();
}
