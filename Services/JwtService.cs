using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using ProjectWork.Models;

namespace ProjectWork.Services;

public class JwtService : IJwtService
{
    private readonly string _jwtKey;
    public JwtService(IConfiguration configuration)
    {
        _jwtKey = configuration["JwtSettings:SecretKey"] ?? throw new ArgumentNullException("JwtSettings not found in configuration.");
    }

    public string GenerateToken(User user)
    {
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user), "User cannot be null.");
        }
        var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtKey);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            }),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
