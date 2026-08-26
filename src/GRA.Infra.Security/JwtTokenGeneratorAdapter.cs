using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using GRA.Domain.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace GRA.Infra.Security;

public class JwtTokenGeneratorAdapter : IJwtTokenGenerator
{
    private readonly IConfiguration _configuration;

    public JwtTokenGeneratorAdapter(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GerarToken(IEnumerable<Claim> claims)
    {
        var chave = _configuration["Jwt:Key"]
            ?? throw new InvalidOperationException("Jwt:Key não configurada.");
        var issuer = _configuration["Jwt:Issuer"];
        var audience = _configuration["Jwt:Audience"];
        var expiracaoMinutos = int.Parse(_configuration["Jwt:ExpiracaoMinutos"] ?? "60");

        var chaveBytes = Encoding.UTF8.GetBytes(chave);
        var credenciais = new SigningCredentials(
            new SymmetricSecurityKey(chaveBytes),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiracaoMinutos),
            signingCredentials: credenciais);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}