namespace Net10Auth.Shared.Infrastructure;

public class JwtConfiguration
{
    
    public required string[] ValidAudiences { get; set; }
    public required string ValidIssuer { get; set; }
    public required string SecurityKey { get; set; }
    public required string AccessTokenExpiryInSeconds { get; set; }
    public required int RefreshTokenExpiryInHours { get; set; }
    public required int MaxRefreshTokenAttempts { get; set; }
}