namespace Net10Auth.Shared.Infrastructure.Functional.Errors;

public static class ResultErrorFactory
{
    public static BasicResultError BasicError(string? message = "something went wrong") => new(message);
    
}