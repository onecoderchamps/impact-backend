using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

public class JwtService
{
    private readonly string key;

    public JwtService(IConfiguration configuration)
    {
        this.key = configuration.GetSection("AppSettings")["JwtKey"];
    }

    public string GenerateJwtToken(string id, object uuid)
    {
        var tokenDescriptor = CreateTokenDescriptor(id, uuid);
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    private SecurityTokenDescriptor CreateTokenDescriptor(string id, object uuid)
    {
        var keys = Encoding.ASCII.GetBytes(key);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, uuid?.ToString() ?? string.Empty), // NOTE: this will be the "User.Identity.Name" value
                new Claim(JwtRegisteredClaimNames.Sub, id),
            }),
            Expires = DateTime.UtcNow.AddDays(30),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(keys), SecurityAlgorithms.HmacSha256Signature),
            Issuer = "impact.com",
            Audience = "impact.com",
        };

        return tokenDescriptor;
    }
}