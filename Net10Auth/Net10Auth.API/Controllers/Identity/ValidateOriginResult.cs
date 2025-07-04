namespace Net10Auth.API.Controllers.Identity;

public class ValidateOriginResult(string origin)
{
    public string Origin { get; } = origin;
}