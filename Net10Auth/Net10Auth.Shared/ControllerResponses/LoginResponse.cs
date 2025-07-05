// ReSharper disable MemberCanBePrivate.Global

namespace Net10Auth.Shared.ControllerResponses;

public class LoginResponse 
{
    public LoginResponse()
    {
        // do not remove
    }

    public LoginResponse(string? accessToken, string refreshToken, DateTime validTo, bool twoFactorEnabled, string? token = null) : this()
    {
        AccessToken = accessToken;
        RefreshToken = refreshToken;
        ValidTo = validTo;
        TwoFactorEnabled = twoFactorEnabled;
        Token = token;
    }

    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime ValidTo { get; set; }
    public bool TwoFactorEnabled { get; set; }
    public string? Token { get; set; }


}

public class ControllerResponseError
{
}

public interface IControllerResponse
{
    public int StatusCode { get; set; }
    public string? Message { get; set; }
    public IEnumerable<ControllerResponseError>? Errors { get; set; }
}