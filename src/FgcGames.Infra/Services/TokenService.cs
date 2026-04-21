using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FgcGames.Application.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace FgcGames.Infra.Services;

public class TokenService : ITokenService
{
    public string GenerateJwtToken(string email, string role)
    {
        const string issuer = "FgcGames-Issuer";
        const string securityKey = "FgcGames_Secret_Key_2026_High_Security_Token";

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, email),
            new Claim(ClaimTypes.Role, role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: null,
            claims: claims,
            expires: DateTime.Now.AddHours(2),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
