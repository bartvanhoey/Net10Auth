// ReSharper disable MemberCanBePrivate.Global

namespace Net10Auth.Shared.ControllerResponses;

public class LoginResponse(
    string? accessToken,
    string refreshToken,
    DateTime validTo,
    bool twoFactorEnabled,
    string? userId = null)
{
    public string? AccessToken { get; set; } = accessToken;
    public string? RefreshToken { get; set; } = refreshToken;
    public DateTime ValidTo { get; set; } = validTo;
    public bool TwoFactorEnabled { get; set; } = twoFactorEnabled;
    public string? UserId { get; set; } = userId;
}