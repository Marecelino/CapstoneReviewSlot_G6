using Identity.Application.Abstractions.Security;
using Identity.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Identity.Infrastructure.Security;

public class JwtTokenService : IJwtTokenService
{
    private readonly IConfiguration _configuration;

    public JwtTokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Sinh JWT cho user. Nếu user là Lecturer thì thêm claim "lecturer_id".
    /// </summary>
    public string GenerateToken(User user, Guid? lecturerId = null)
    {
        var key = _configuration["Jwt:Key"]
                  ?? throw new InvalidOperationException("Jwt:Key is not configured.");

        var issuer = _configuration["Jwt:Issuer"]
                     ?? throw new InvalidOperationException("Jwt:Issuer is not configured.");

        var audience = _configuration["Jwt:Audience"]
                       ?? throw new InvalidOperationException("Jwt:Audience is not configured.");

        var expiryMinutes = int.TryParse(_configuration["Jwt:ExpiryMinutes"], out var val) ? val : 60;

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub,   user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti,   Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.NameIdentifier,     user.Id.ToString()),
            new Claim(ClaimTypes.Name,               user.FullName),
            new Claim(ClaimTypes.Role,               user.Role.ToString())
        };

        if (lecturerId.HasValue)
        {
            claims.Add(new Claim("lecturer_id", lecturerId.Value.ToString()));
        }

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer:            issuer,
            audience:          audience,
            claims:            claims,
            expires:           DateTime.UtcNow.AddMinutes(expiryMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public bool ValidateToken(string token)
    {
        try
        {
            var key = _configuration["Jwt:Key"] ?? throw new InvalidOperationException();
            new JwtSecurityTokenHandler().ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                ValidateIssuer = true,
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidateAudience = true,
                ValidAudience = _configuration["Jwt:Audience"],
                ClockSkew = TimeSpan.Zero
            }, out _);
            return true;
        }
        catch { return false; }
    }

    public Guid? GetUserIdFromToken(string token)
    {
        try
        {
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
            var sub = jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
            return sub is not null && Guid.TryParse(sub, out var id) ? id : null;
        }
        catch { return null; }
    }

    public Guid? GetLecturerIdFromToken(string token)
    {
        try
        {
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
            var val = jwt.Claims.FirstOrDefault(c => c.Type == "lecturer_id")?.Value;
            return val is not null && Guid.TryParse(val, out var id) ? id : null;
        }
        catch { return null; }
    }
}