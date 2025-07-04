namespace Net10Auth.API.Controllers.Identity;

public class RegisterResponse(string? code, string? userId)
{
    public string? Code { get; set; } = code;
    public string? UserId { get; set; } = userId;
}