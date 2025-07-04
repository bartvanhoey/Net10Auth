using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Net10Auth.API.Database;
using Net10Auth.Shared.Infrastructure.Extensions;
using Net10Auth.Shared.Infrastructure.Functional;
using Net10Auth.Shared.Models;
using static System.String;

namespace Net10Auth.API.Controllers.Identity;

public class AuthControllerBase(
    UserManager<ApplicationUser> userManager,
    RoleManager<IdentityRole> roleManager,
    IConfiguration configuration,
    IHostEnvironment env)
    : CustomControllerBase()
{
    protected readonly IConfiguration Configuration = configuration;
    protected readonly RoleManager<IdentityRole> RoleManager = roleManager;
    protected readonly UserManager<ApplicationUser> UserManager = userManager;

    protected Result<ValidateControllerResult> ValidateControllerInputModel<T>(BaseInputModel? input, ILogger<T> logger,
        string methodName)
    {
        if (input is not null) return ValidateController(logger, methodName);
        logger.LogError("{MethodName}: input is null", methodName);
        return Result.Failure<ValidateControllerResult>("input is null");
    }

    protected Result<ValidateControllerResult> ValidateController<T>(ILogger<T> logger, string methodName)
    {
        var securityKey = Configuration["Jwt:SecurityKey"];
        if ( IsNullOrEmpty(securityKey))
        {
            logger.LogError("{MethodName}: security key is null", methodName);
            return Result.Failure<ValidateControllerResult>("security key is null");
        }

        var validIssuer = Configuration["Jwt:ValidIssuer"];
        if (IsNullOrEmpty(validIssuer))
        {
            logger.LogError("{MethodName}: valid issuer is null", methodName);
            return Result.Failure<ValidateControllerResult>("valid issuer is null");
        }

        var originResult = ValidateOrigin(logger, methodName);
        return originResult.IsSuccess
            ? Result.Success(new ValidateControllerResult(securityKey, validIssuer, originResult.Value.Origin))
            : Result.Failure<ValidateControllerResult>(originResult.Error);
    }

    protected Result<ValidateOriginResult> ValidateOrigin<T>(ILogger<T> logger, string methodName)
    {
        var validAudiences = Configuration.GetSection("Jwt:ValidAudiences").Get<List<string>>();

        if (validAudiences == null || validAudiences.Count == 0)
        {
            logger.LogError("{MethodName}: audience is null", methodName);
            return Result.Failure<ValidateOriginResult>("audience is null");
        }

        var origin = HttpContext.Request.Headers.Origin.FirstOrDefault();
        if (origin.IsNotNullOrWhiteSpace() && validAudiences.Contains(origin ?? throw new InvalidOperationException()))
            return Result.Success(new ValidateOriginResult(origin));

        logger.LogError("{MethodName}: origin is wrong: {Origin}", methodName, origin);
        return Result.Failure<ValidateOriginResult>("origin is wrong");
    }
    
    protected IActionResult Nok500CodeIsNull<T>(ILogger logger, [CallerMemberName] string memberName = "")
        where T : IControllerResponse  => Nok500<T>(logger, $"{memberName}-Code is null");
}