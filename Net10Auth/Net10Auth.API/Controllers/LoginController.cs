using Microsoft.AspNetCore.Mvc;
using Net10Auth.API.Controllers.Base;
using Net10Auth.API.Controllers.Identity;
using Net10Auth.API.Services.Login;
using Net10Auth.Shared.ControllerResponses;
using Net10Auth.Shared.Models.Identity;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace Net10Auth.API.Controllers;

[Route("api/account")]
[ApiController]
public class LoginController(ILogger<LoginController> logger, ILoginService loginService) : ControllerBase
{
    [HttpPost]
    [Route("login")]
    [ProducesResponseType(Status200OK, Type = typeof(LoginResponse))]
    [ProducesResponseType(Status500InternalServerError)]
    [ProducesResponseType(Status423Locked)]
    public async Task<IActionResult> Login([FromBody] LoginInputModel model)
    {
        var response = (await loginService.LoginAsync(model, Request)).IfFailureLogErrorMessage(logger);
        return response.IsSuccess ? Ok(response) : BadRequest(response);
    }
}