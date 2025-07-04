using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Net10Auth.API.Database;
using Net10Auth.Shared.Infrastructure.Extensions;
using Net10Auth.Shared.Models.Identity;
using Serilog.Context;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace Net10Auth.API.Controllers.Identity;

[Route("api/account")]
[ApiController]
public class LoginController(
    UserManager<ApplicationUser> userManager,
    RoleManager<IdentityRole> roleManager,
    SignInManager<ApplicationUser> signInManager,
    IHostEnvironment environment,
    IConfiguration configuration,
    ILogger<LoginController> logger)
#pragma warning disable CS9107 // Parameter is captured into the state of the enclosing type and its value is also passed to the base constructor. The value might be captured by the base class as well.
    : AuthControllerBase(userManager, roleManager, configuration, environment)
#pragma warning restore CS9107 // Parameter is captured into the state of the enclosing type and its value is also passed to the base constructor. The value might be captured by the base class as well.
{
    private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
    

    [HttpPost]
    [Route("login")]
    [ProducesResponseType(Status200OK, Type = typeof(LoginResponse))]
    [ProducesResponseType(Status500InternalServerError)]
    [ProducesResponseType( Status423Locked)]
    public async Task<IActionResult> Login([FromBody] LoginInputModel? model)
    {
        try
        {
            var validationResult = ValidateControllerInputModel(model, logger, nameof(Login));
            if (validationResult.IsFailure) return Nok500<LoginResponse>(logger, validationResult.Error);

            var user = await userManager.FindByEmailAsync(model?.Email ?? string.Empty);
            if (user == null) return Nok404CouldNotFindUser<LoginResponse>(logger);

            if (string.IsNullOrWhiteSpace(user.Email)) return Nok400Email<LoginResponse>(logger);
            if (string.IsNullOrWhiteSpace(model?.Password)) return Nok400Password<LoginResponse>(logger);
            
            var signInResult = await _signInManager.CheckPasswordSignInAsync(user, model.Password ?? throw new InvalidOperationException(),  true);
            if (signInResult is { Succeeded: false, IsLockedOut: true } ) return Locked423<LoginResponse>(logger );
            
            if (!signInResult.Succeeded) return Nok500<LoginResponse>(logger, "Invalid login attempt");

            user.SetRefreshToken(configuration);

            await userManager.UpdateAsync(user);

             var (accessToken, validTo) = await user.GenerateAccessToken(UserManager, Configuration,
                 validationResult.Value.ValidIssuer, validationResult.Value.Origin, validationResult.Value.SecurityKey);

            if (user.RefreshToken == null || user.RefreshToken.IsNullOrWhiteSpace())
                return Nok500RefreshToken<LoginResponse>(logger);
            
            if (accessToken.IsNullOrWhiteSpace()) return Nok500<LoginResponse>(logger, "AccessToken is null or whitespace");
            if (validTo <= DateTime.UtcNow) return Nok500<LoginResponse>(logger, "ValidTo is in the past" );

            LogContext.PushProperty("email", "john@example.com");
            
            return Ok(user.TwoFactorEnabled 
                ? new LoginResponse(accessToken, user.RefreshToken, validTo, user.TwoFactorEnabled, user.Id) 
                : new LoginResponse(accessToken, user.RefreshToken, validTo, user.TwoFactorEnabled));
        }
        catch (Exception exception) { return Nok500<LoginResponse>(logger, exception); }
    }

}