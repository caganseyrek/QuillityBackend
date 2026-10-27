using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Quillity.Auth.Managers;

public class TokenManager
{
    private readonly string Issuer;
    private readonly string Audience;
    private readonly string AccessTokenKey;
    private readonly string RefreshTokenKey;

    public TokenManager(IConfiguration configuration)
    {
        Issuer = configuration["JWT:Issuer"] ?? throw new Exception("JWT issuer value is invalid");
        Audience = configuration["JWT:Audience"] ?? throw new Exception("JWT audience value is invalid");
        AccessTokenKey = configuration["JWT:AccessTokenKey"] ?? throw new Exception("JWT access token key value is invalid");
        RefreshTokenKey = configuration["JWT:RefreshTokenKey"] ?? throw new Exception("JWT refresh token key value is invalid");
    }

    public string GenerateAccessToken(string userId, string email) => GenerateToken(userId, email, AccessTokenKey);

    public string GenerateRefreshToken(string userId, string email) => GenerateToken(userId, email, RefreshTokenKey);

    private string GenerateToken(string userId, string email, string key)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim(ClaimTypes.Email, email),
        };

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
        var tokenDescriptor = new JwtSecurityToken(
            issuer: Issuer,
            audience: Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(16),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
    }
}
