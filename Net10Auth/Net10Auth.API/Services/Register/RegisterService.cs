using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Net10Auth.API.Controllers;
using Net10Auth.API.Controllers.Identity;
using Net10Auth.API.Database;
using Net10Auth.Shared.Infrastructure.Extensions;
using Net10Auth.Shared.Models.Identity;

namespace Net10Auth.API.Services.Register;

public class RegisterService(
    UserManager<ApplicationUser> userManager,
    IConfiguration configuration,
    IEmailSender<ApplicationUser> emailSender,
    RoleManager<IdentityRole> roleManager) : IRegisterService
{
    public async Task<CtrlResponse<RegisterResponse>> RegisterAsync(RegisterInputModel model, HttpRequest request)
    {
        var callbackUrl = $"{request.Headers.Origin.FirstOrDefault()}/account/confirm-email";
        if (IsNullOrEmpty(model.Email)) return new CtrlResponse<RegisterResponse>("Email is null");

        if (IsNullOrEmpty(model.Password)) return new CtrlResponse<RegisterResponse>("Password is null");

        var user = await userManager.FindByEmailAsync(model.Email);
        if (user != null) return new CtrlResponse<RegisterResponse>($"user '{model.Email}' already exists");

        var newUser = CreateApplicationUser();
        if (newUser == null) return new CtrlResponse<RegisterResponse>("New user is null");

        newUser.Email = model.Email;
        newUser.UserName = model.Email;

        var result = await userManager.CreateAsync(newUser, model.Password);
        if (!result.Succeeded)
            return new CtrlResponse<RegisterResponse>(result.Errors.Any()
                ? Join("; ", result.Errors.Select(e => e.Code))
                : "Unable to register user due to unknown errors.");

        // await UserManager.SetTwoFactorEnabledAsync(newUser, true);
        await newUser.AddToUserRoleAsync(userManager, roleManager);
        await newUser.AddToAdminRoleIfAdministratorAsync(userManager, roleManager, configuration);

        var userId = await userManager.GetUserIdAsync(newUser);
        if (userId.IsNullOrWhiteSpace()) return new CtrlResponse<RegisterResponse>("User id is null");

        var code = await userManager.GenerateEmailConfirmationTokenAsync(newUser);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

        var confirmationLink = callbackUrl.AddUrlParameters(new Dictionary<string, object?>
            { ["token"] = userId, ["code"] = code, ["returnUrl"] = null });

        await emailSender.SendConfirmationLinkAsync(newUser, newUser.Email, confirmationLink);
        
        
        
        return new CtrlResponse<RegisterResponse>(new RegisterResponse(code, userId));
    }

    private static ApplicationUser? CreateApplicationUser()
    {
        try
        {
            return Activator.CreateInstance<ApplicationUser>();
        }
        catch
        {
            return null;
        }
    }
}