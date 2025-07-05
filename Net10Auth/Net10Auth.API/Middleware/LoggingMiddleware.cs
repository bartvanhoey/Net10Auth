using System.Diagnostics;
using System.Text.Json;
using Microsoft.AspNetCore.Http.Features;
using Net10Auth.API.Controllers;
using Net10Auth.Shared.ControllerResponses;
using Net10Auth.Shared.Infrastructure;
using Serilog.Context;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace Net10Auth.API.Middleware
{
    public class LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger, ITokenHandler tokenHandler)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            var endpoint = context.Features.Get<IEndpointFeature>()?.Endpoint;
            if (endpoint == null)
            {
                await next(context);
                return;
            }

            var watcher = new Stopwatch();
            watcher.Start();

            try
            {
                watcher.Stop();
                using (LogContext.PushProperty("UserEmail", context.GetUserEmailFromAuthorizationHeader(tokenHandler)))
                using (LogContext.PushProperty("UserIP", context.Connection.RemoteIpAddress))
                using (LogContext.PushProperty("Elapsed", watcher.ElapsedMilliseconds))
                {
                    var message = $"{nameof(LoggingMiddleware)} - StatusCode {context.Response.StatusCode}, UserEmail: {context.GetUserEmailFromAuthorizationHeader(tokenHandler)}, UserIP: {context.Connection.RemoteIpAddress}, Request took {watcher.ElapsedMilliseconds} ms.";
                    if (context.Response.StatusCode < 400) logger.LogInformation(message);
                    else logger.LogError(message);
                }
                // Call the next middleware in the pipeline
                await next(context);
            }
            catch (Exception ex)
            {
                // Log any exceptions that occur during request processing
                watcher.Stop();
                using (LogContext.PushProperty("UserEmail", context.GetUserEmailFromAuthorizationHeader(tokenHandler)))
                using (LogContext.PushProperty("UserIP", context.Connection.RemoteIpAddress))
                using (LogContext.PushProperty("Elapsed", watcher.ElapsedMilliseconds))
                {
                    var message = $"{nameof(LoggingMiddleware)} - Exception: {ex.Message}, UserEmail: {context.GetUserEmailFromAuthorizationHeader(tokenHandler)}, UserIP: {context.Connection.RemoteIpAddress}, Request took {watcher.ElapsedMilliseconds} ms.";
                    logger.LogError(ex, message);
                }

                if (context.Response.HasStarted) throw;
                context.Response.Clear();
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = Status500InternalServerError;
                var responseModel = new CtrlResponse<object>("An internal server error occurred");
                var json = JsonSerializer.Serialize(responseModel, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });
                await context.Response.WriteAsync(json);
            }
        }
    }
}