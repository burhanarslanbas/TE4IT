using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using TE4IT.Application.Abstractions.Auth;

namespace TE4IT.Infrastructure.Auth.Services;

public sealed class JwtTokenService(IConfiguration configuration) : ITokenService
{
    public (string accessToken, DateTime expiresAt) CreateAccessToken(Guid userId, string userName, string email, IEnumerable<string> roles, IEnumerable<string> permissions, string? permissionsVersion = null)
    {
        // Azure App Service için environment variables'dan JWT settings oku
        var issuer = configuration["Jwt:Issuer"] ?? configuration["JWT_ISSUER"]!;
        var audience = configuration["Jwt:Audience"] ?? configuration["JWT_AUDIENCE"]!;
        var signingKey = JwtKeyHelper.CreateSymmetricKey(
            configuration["Jwt:SigningKey"] ?? configuration["JWT_SIGNING_KEY"]!);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new(ClaimTypes.NameIdentifier, userId.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.UniqueName, userName ?? string.Empty),
            new(JwtRegisteredClaimNames.Email, email ?? string.Empty)
        };
        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));
        claims.AddRange(permissions.Select(p => new Claim("permission", p)));
        if (!string.IsNullOrWhiteSpace(permissionsVersion))
        {
            claims.Add(new Claim("permissions_version", permissionsVersion!));
        }

        var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddMinutes(15); // ✅ 15 dakika (güvenlik için kısa süre)

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: expires,
            signingCredentials: creds
        );

        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.WriteToken(token);
        return (jwt, expires);
    }

    public (string refreshToken, DateTime expiresAt) CreateRefreshToken(Guid userId)
    {
        var bytes = RandomNumberGenerator.GetBytes(64);
        var token = Convert.ToBase64String(bytes);
        return (token, DateTime.UtcNow.AddDays(7));
    }
}

internal static class JwtKeyHelper
{
    public static SymmetricSecurityKey CreateSymmetricKey(this string signingKey)
    {
        // If the configured key is Base64, decode it; otherwise use UTF8 bytes of the text
        try
        {
            var raw = Convert.FromBase64String(signingKey);
            return new SymmetricSecurityKey(raw);
        }
        catch
        {
            return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey));
        }
    }
}

