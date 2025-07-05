using System.Diagnostics;
using System.Text.Json;
using Microsoft.AspNetCore.Http.Features;
using Net10Auth.API.Controllers;
using Net10Auth.Shared.Infrastructure;
using Serilog.Context;

namespace Net10Auth.API.Middleware
{
    public class LoggingMiddleware(
        RequestDelegate next,
        ILogger<LoggingMiddleware> logger,
        ITokenHandler tokenHandler
    )
    {
        public async Task InvokeAsync(HttpContext context)
        {
            var endpoint = context.Features.Get<IEndpointFeature>()?.Endpoint;
            if (endpoint == null)
            {
                await next(context);
                return;
            }

            var userEmail = string.Empty;
            var userIp = context.Connection.RemoteIpAddress;

            // Extract JWT token from the Authorization header
            var authorizationHeader = context.Request.Headers.Authorization.FirstOrDefault();
            if (!string.IsNullOrEmpty(authorizationHeader) && authorizationHeader.StartsWith("Bearer "))
            {
                var token = authorizationHeader["Bearer ".Length..].Trim();
                userEmail = tokenHandler.GetEmailFromToken(token);
            }

            var watcher = new Stopwatch();
            watcher.Start();

            try
            {
                watcher.Stop();

                using (LogContext.PushProperty("UserEmail", userEmail))
                using (LogContext.PushProperty("UserIP", userIp))
                using (LogContext.PushProperty("Elapsed", watcher.ElapsedMilliseconds))
                {
                    var message = $"{nameof(LoggingMiddleware)} - " +
                                  $"Status code {context.Response.StatusCode}, " +
                                  $"UserEmail: {userEmail}, " +
                                  $"UserIP: {userIp}, " +
                                  $"Request took {watcher.ElapsedMilliseconds} ms.";

                    if (context.Response.StatusCode < 400)
                    {
                        logger.LogInformation(message);
                    }
                    else
                    {
                        logger.LogError(message);
                    }
                }

                // Call the next middleware in the pipeline
                await next(context);
            }
            catch (Exception ex)
            {
                // Log any exceptions that occur during request processing
                watcher.Stop();

                using (LogContext.PushProperty("UserEmail", userEmail))
                using (LogContext.PushProperty("UserIP", userIp))
                using (LogContext.PushProperty("Elapsed", watcher.ElapsedMilliseconds))
                {
                    logger.LogError(ex, $"{nameof(LoggingMiddleware)} - " +
                                        $"Exception: {ex.Message}, " +
                                        $"UserEmail: {userEmail}, " +
                                        $"UserIP: {userIp}, " +
                                        $"Request took {watcher.ElapsedMilliseconds} ms.");
                }

                if (context.Response.HasStarted)
                    throw;

                context.Response.Clear();
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;

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