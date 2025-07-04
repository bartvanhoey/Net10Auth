namespace Net10Auth.API.Controllers.Identity;

public class ValidateControllerResult(
    string securityKey,
    string validIssuer,
    string origin)
{
    public string SecurityKey { get; } = securityKey;
    public string ValidIssuer { get; } = validIssuer;
    public string Origin { get; } = origin;
}