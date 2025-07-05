namespace Net10Auth.Shared.ControllerResponses;

public class RegisterResponse(string? code, string? userId)
{
    public string? Code { get; set; } = code;
    public string? UserId { get; set; } = userId;
}