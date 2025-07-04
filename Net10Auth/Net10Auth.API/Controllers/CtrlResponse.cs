namespace Net10Auth.API.Controllers;

public class CtrlResponse<T>(bool isSuccess, string? message, T? data)
{
    public CtrlResponse(string? message) : this(false, message, default)
    {
    }
    
    public CtrlResponse(T data) : this(true, "", data)
    {
    }

    public T? Data { get; set; } = data;
    public bool IsSuccess { get; set; } = isSuccess;
    public bool IsFailure => !IsSuccess;
    public string? Message { get; set; } = message;
}