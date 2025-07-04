using static Microsoft.Extensions.Logging.LogLevel;

// ReSharper disable TemplateIsNotCompileTimeConstantProblem

namespace Net10Auth.API.Controllers.Identity;

public static class LoggerExtensions
{
    public static void Log(this ILogger logger, string memberName, Exception? exception, LogLevel? logLevel = Error,
        string? errorMessage = "something went wrong")
    {
        if (exception == null)
            switch (logLevel)
            {
                case Information:
                    logger.LogInformation("{MemberName} : {ErrorMessage}", memberName, errorMessage);
                    break;
                case Warning:
                    logger.LogWarning("{MemberName} : {ErrorMessage}", memberName, errorMessage);
                    break;
                case Error:
                    logger.LogError("{MemberName} : {ErrorMessage}", memberName, errorMessage);
                    break;
                case Critical:
                    logger.LogCritical("{MemberName} : {ErrorMessage}", memberName, errorMessage);
                    break;
                case Trace:
                    logger.LogTrace("{MemberName} : {ErrorMessage}", memberName, errorMessage);
                    break;
                case Debug:
                    logger.LogDebug("{MemberName} : {ErrorMessage}", memberName, errorMessage);
                    break;
                default:
                    logger.LogInformation("{MemberName} : {ErrorMessage}", memberName, errorMessage);
                    break;
            }
        else
            logger.LogError(exception, memberName);
    }

    
}