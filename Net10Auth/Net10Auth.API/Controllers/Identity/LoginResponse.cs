using static Microsoft.AspNetCore.Http.StatusCodes;

// ReSharper disable MemberCanBePrivate.Global

namespace Net10Auth.API.Controllers.Identity;

public class LoginResponse : IControllerResponse
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
        StatusCode = Status200OK;
    }

    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime ValidTo { get; set; }
    public bool TwoFactorEnabled { get; set; }

    public int StatusCode { get; set; }
    public string? Message { get; set; }
    
    public string? Token { get; set; }

    public IEnumerable<ControllerResponseError>? Errors { get; set; }
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