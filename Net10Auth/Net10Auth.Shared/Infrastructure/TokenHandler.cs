using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Net10Auth.Shared.Infrastructure;

public class TokenHandler(IOptions<JwtConfiguration> tokenConfig) : ITokenHandler
{
    private readonly JwtConfiguration _tokenConfig = tokenConfig.Value;

    public string GenerateJwtToken(List<Claim> claims, string validAudience)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenConfig.SecurityKey));
        var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var tokenValidity = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_tokenConfig.AccessTokenExpiryInSeconds));

        var token = new JwtSecurityToken(
            issuer: _tokenConfig.ValidIssuer,
            audience: validAudience,
            claims: claims,
            expires: tokenValidity,
            signingCredentials: signingCredentials);

        var tokenHandler = new JwtSecurityTokenHandler();
        return tokenHandler.WriteToken(token);
    }

    public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token, string validAudience)
    {
        var tokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidIssuer = _tokenConfig.ValidIssuer,
            ValidateAudience = true,
            ValidAudience = validAudience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenConfig.SecurityKey)),
            ValidateLifetime = false // <— allows expired token
        };
        
        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
        var jwtSecurityToken = securityToken as JwtSecurityToken;
        if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            throw new SecurityTokenException("Invalid token");
        return principal;
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    public int GetRefreshTokenExpiryDays() => _tokenConfig.RefreshTokenExpiryInHours;

    public int GetMaxRefreshTokenAttempts() => _tokenConfig.MaxRefreshTokenAttempts;
    
    public string GetEmailFromToken(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);
        var email = jwtToken.Claims.First(claim => claim.Type == "Email").Value;
        return email;
    }
}