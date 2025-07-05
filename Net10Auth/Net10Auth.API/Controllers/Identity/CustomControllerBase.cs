using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Net10Auth.Shared.ControllerResponses;
using Net10Auth.Shared.Infrastructure.Extensions;
using Net10Auth.Shared.Infrastructure.Functional.Errors;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace Net10Auth.API.Controllers.Identity;

public class CustomControllerBase : ControllerBase
{
    private static IActionResult GetControllerResponse<T>(int statusCode, string? message = null)
        where T : IControllerResponse
    {
        if (statusCode == Status500InternalServerError)
            return new StatusCodeResult(Status500InternalServerError);

        if (Activator.CreateInstance(typeof(T)) is not IControllerResponse response)
            return new StatusCodeResult(Status500InternalServerError);

        response.Message = message;

        return new ObjectResult(response) { StatusCode = statusCode };
    }
    
    
    protected IActionResult Ok200<T>(string? message = null)
    {
#pragma warning disable CA2263
        var controllerResponse = Activator.CreateInstance(typeof(T)) as IControllerResponse;
#pragma warning restore CA2263
        if (controllerResponse == null) return Ok();
        controllerResponse.Message = message.IsNullOrWhiteSpace() ? "success" : message;
        return Ok(controllerResponse);
    }

    protected IActionResult Locked423<T>(ILogger logger, LogLevel level = LogLevel.Error,
        [CallerMemberName] string member = "callMemberUnknown") where T : IControllerResponse
    {
        logger.Log(member, null, level, "Account is locked");
        return GetControllerResponse<T>(Status423Locked);
    }

    protected IActionResult Nok500<T>(ILogger logger, string? errorMessage = "something went wrong",
        LogLevel level = LogLevel.Error,
        [CallerMemberName] string member = "callMemberUnknown") where T : IControllerResponse
    {
        logger.Log(member, null, level, errorMessage);
        return GetControllerResponse<T>(Status500InternalServerError);
    }

    protected IActionResult Nok500<T>(ILogger logger, BaseResultError? validationResultError, LogLevel level = LogLevel.Error,
        [CallerMemberName] string member = "callMemberUnknown") where T : IControllerResponse
    {
        logger.Log(member, null, level, string.Join(";", validationResultError?.Messages ?? [""]));
        return GetControllerResponse<T>(Status500InternalServerError);
    }
    

    protected IActionResult Nok500<T>(ILogger logger, IEnumerable<IdentityError>? errors,
        [CallerMemberName] string member = "callMemberUnknown") where T : IControllerResponse
    {
        logger.Log(member, null, LogLevel.Error, errors == null
            ? "Errors is null"
            :  string.Join(";", errors.Select(x => $"{x.Code} - {x.Description}").ToList()));
        return GetControllerResponse<T>(Status500InternalServerError);
    }

    protected IActionResult Nok500<T>(ILogger logger, Exception exception,
        [CallerMemberName] string member = "callMemberUnknown") where T : IControllerResponse
    {
        logger.Log(member, exception);
        return GetControllerResponse<T>(Status500InternalServerError);
        
    }

    private IActionResult Nok404<T>(ILogger logger, string? errorMessage = "something went wrong",
        LogLevel logLevel = LogLevel.Error,
        [CallerMemberName] string member = "callMemberUnknown") where T : IControllerResponse
    {
        logger.Log(member, null, logLevel, errorMessage);
        return GetControllerResponse<T>(Status404NotFound);
    }

    private IActionResult Nok400<T>(ILogger logger, string? errorMessage = "something went wrong",
        LogLevel level = LogLevel.Error,
        [CallerMemberName] string member = "callMemberUnknown") where T : IControllerResponse
    {
        logger.Log(member, null, level, errorMessage);
        return GetControllerResponse<T>(Status400BadRequest);
    }

    protected IActionResult Nok500RoleIsNullOrWhiteSpace<T>(ILogger logger,
        [CallerMemberName] string member = "callMemberUnknown")
        where T : IControllerResponse =>
        Nok500<T>(logger, "Role is null or whitespace", LogLevel.Warning, member);

    protected IActionResult Nok404CouldNotFindUser<T>(ILogger logger,
        [CallerMemberName] string member = "callMemberUnknown")
        where T : IControllerResponse =>
        Nok404<T>(logger, "Could not find user", LogLevel.Warning, member);

    protected IActionResult Nok400Email<T>(ILogger logger, [CallerMemberName] string member = "callMemberUnknown")
        where T : IControllerResponse =>
        Nok400<T>(logger, "Email is null or white space", LogLevel.Warning, member);

    protected IActionResult Nok400Password<T>(ILogger logger, [CallerMemberName] string member = "callMemberUnknown")
        where T : IControllerResponse =>
        Nok400<T>(logger, "Password is null or white space", LogLevel.Warning, member);

    protected IActionResult Nok400UserIdsAreNotTheSame<T>(ILogger logger,
        [CallerMemberName] string member = "callMemberUnknown")
        where T : IControllerResponse =>
        Nok400<T>(logger, "User ids are not the same", LogLevel.Warning, member);

    protected IActionResult Nok500RoleDoesNotExist<T>(ILogger logger,
        [CallerMemberName] string member = "callMemberUnknown")
        where T : IControllerResponse =>
        Nok500<T>(logger, "User role does not exist", LogLevel.Warning, member);

    protected IActionResult Nok500AccessToken<T>(ILogger logger, [CallerMemberName] string member = "callMemberUnknown")
        where T : IControllerResponse =>
        Nok500<T>(logger, "Access token is null or white space", LogLevel.Warning, member);

    protected IActionResult Nok500RefreshToken<T>(ILogger logger,
        [CallerMemberName] string member = "callMemberUnknown")
        where T : IControllerResponse =>
        Nok500<T>(logger, "Refresh token is null or white space", LogLevel.Warning, member);

    protected IActionResult Nok500WrongRefreshToken<T>(ILogger logger,
        [CallerMemberName] string member = "callMemberUnknown")
        where T : IControllerResponse =>
        Nok500<T>(logger, "Something wrong with Refresh token", LogLevel.Warning, member);

    protected IActionResult Nok500Principal<T>(ILogger logger, [CallerMemberName] string member = "callMemberUnknown")
        where T : IControllerResponse =>
        Nok500<T>(logger, "Principal is null", LogLevel.Warning, member);

    protected IActionResult Nok500EmailFromRequestIsNullOrWhiteSpace<T>(ILogger logger,
        BaseResultError? error,
        [CallerMemberName] string member = "callMemberUnknown") where T : IControllerResponse =>
        Nok500<T>(logger, string.Join(";", error?.Messages ?? [""]) , LogLevel.Warning, member);

    protected IActionResult Nok500CannotRemoveOwnAdminRole<T>(ILogger logger,
        [CallerMemberName] string member = "callMemberUnknown")
        where T : IControllerResponse =>
        Nok500<T>(logger, "An admin user cannot remove his own admin role", LogLevel.Warning, member);


}