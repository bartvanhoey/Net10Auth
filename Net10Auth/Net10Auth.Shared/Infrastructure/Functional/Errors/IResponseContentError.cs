using System.Net;

namespace Net10Auth.Shared.Infrastructure.Functional.Errors;

public class ResponseContentError(string? errorMessage, HttpStatusCode? statusCode = null) : BaseResultError(errorMessage)
{
    public HttpStatusCode? StatusCode { get; } = statusCode;
}