using System.Security.Claims;

namespace Net10Auth.Shared.Infrastructure
{
    // public interface ITokenHandler
    // {
    //     string GenerateJwtToken(List<Claim> claims);
    //     ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
    //     string GenerateRefreshToken();
    //     int GetRefreshTokenExpiryDays();
    //     int GetMaxRefreshTokenAttempts();
    // }
    
    public interface ITokenHandler
    {
        string GenerateJwtToken(List<Claim> claims, string validAudience);
        ClaimsPrincipal? GetPrincipalFromExpiredToken(string token, string validAudience);
        string GenerateRefreshToken();
        int GetRefreshTokenExpiryDays();
        int GetMaxRefreshTokenAttempts();
        string GetEmailFromToken(string token);
    }
}
