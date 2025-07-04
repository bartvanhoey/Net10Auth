namespace Net10Auth.Shared.Infrastructure.Models;

public interface IHaveISuccessAndErrorMessage
{
    public bool IsSuccess { get; set; }
    public string? ErrorMessage { get; set; }
}