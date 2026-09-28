using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace ToDoKits.Controllers;

/// <summary>签发 JWT（HS256）。密钥来自配置 Jwt:Secret，缺省用开发用密钥。</summary>
public static class JwtHelper
{
    public static string CreateToken(long userId, string username, string secret)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Name, username),
        };
        var token = new JwtSecurityToken(
            issuer: "todokits",
            audience: "todokits",
            claims: claims,
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: creds);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
