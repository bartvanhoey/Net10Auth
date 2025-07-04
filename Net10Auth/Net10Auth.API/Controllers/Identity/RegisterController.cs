using Microsoft.AspNetCore.Mvc;
using Net10Auth.API.Database;
using Net10Auth.API.Services.Register;
using Net10Auth.Shared.Models.Identity;
using static System.Activator;

namespace Net10Auth.API.Controllers.Identity;

public static class CtrlResponseExtensions
{
    public static CtrlResponse<TR> IfFailureLogErrorMessage<T, TR>(this CtrlResponse<TR> response,
        ILogger<T> logger)
    {
        if (response.IsSuccess) return response;
        logger.LogError(response.Message);
        return response;
    }
}

[ApiController]
[Route("api/account")]
public class RegisterController(IRegisterService registerService, ILogger<RegisterController> logger) : ControllerBase
{
    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> Register([FromBody] RegisterInputModel model)
    {
        var response = (await registerService.RegisterAsync(model, Request)).IfFailureLogErrorMessage(logger);
        return response.IsSuccess ? Ok(response) : StatusCode(500);
    }

    // [HttpPost]
    // [Route("register-admin")]
    // public async Task<IActionResult> RegisterAdmin([FromBody] RegisterInputModel model)
    // {
    //     var userExists = await userManager.FindByNameAsync(model.Email);
    //     if (userExists != null)
    //         Nok500<RegisterResponse>(logger, "User already exists");
    //
    //
    //     ApplicationUser user = new ApplicationUser
    //     {
    //         Email = model.Email,
    //         SecurityStamp = Guid.NewGuid().ToString(),
    //         UserName = model.Email
    //     };
    //     var result = await userManager.CreateAsync(user, model.Password);
    //     if (!result.Succeeded) return Nok500<RegisterResponse>(logger, "could not create user");
    //
    //
    //     if (!await roleManager.RoleExistsAsync(UserRoles.Admin))
    //         await roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
    //     if (!await roleManager.RoleExistsAsync(UserRoles.User))
    //         await roleManager.CreateAsync(new IdentityRole(UserRoles.User));
    //
    //     if (await roleManager.RoleExistsAsync(UserRoles.Admin))
    //     {
    //         await userManager.AddToRoleAsync(user, UserRoles.Admin);
    //     }
    //
    //     return Ok200<RegisterResponse>();
    // }


    private static ApplicationUser? CreateApplicationUser()
    {
        try
        {
            return CreateInstance<ApplicationUser>();
        }
        catch
        {
            return null;
        }
    }
}