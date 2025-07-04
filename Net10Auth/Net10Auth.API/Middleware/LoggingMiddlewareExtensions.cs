namespace Net10Auth.API.Middleware;

public static class LoggingMiddlewareExtensions
{
    public static void UseLoggingMiddleware(this IApplicationBuilder builder)
    {
        builder.UseMiddleware<LoggingMiddleware>();
    }
}