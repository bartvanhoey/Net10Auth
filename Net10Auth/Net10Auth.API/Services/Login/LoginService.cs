using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Net10Auth.API.Database;
using Net10Auth.Shared.ControllerResponses;
using Net10Auth.Shared.Infrastructure;
using Net10Auth.Shared.Infrastructure.Extensions;
using Net10Auth.Shared.Models.Identity;

namespace Net10Auth.API.Services.Login;

public class LoginService(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    IConfiguration configuration,
    IOptions<JwtConfiguration> jwtConfiguration) : ILoginService
{
    public async Task<CtrlResponse<LoginResponse>> LoginAsync(LoginInputModel model, HttpRequest httpRequest)
    {
        var user = await userManager.FindByEmailAsync(model.Email);
        if (user == null) return new CtrlResponse<LoginResponse>("user not found");

        if (user.Email.IsNullOrWhiteSpace()) return new CtrlResponse<LoginResponse>("Email is null");

        var signInResult =
            await signInManager.CheckPasswordSignInAsync(user, model.Password ?? throw new InvalidOperationException(),
                true);
        if (signInResult is { Succeeded: false, IsLockedOut: true })
            return new CtrlResponse<LoginResponse>("user is locked out");

        if (!signInResult.Succeeded) return new CtrlResponse<LoginResponse>("Invalid login attempt");

        user.SetRefreshToken(configuration);

        await userManager.UpdateAsync(user);

        var (accessToken, validTo) =
            await user.GenerateAccessToken(userManager, jwtConfiguration.Value, httpRequest);

        if (user.RefreshToken == null || user.RefreshToken.IsNullOrWhiteSpace())
            return new CtrlResponse<LoginResponse>("refresh token is null");

        if (accessToken.IsNullOrWhiteSpace())
            return new CtrlResponse<LoginResponse>("AccessToken is null or whitespace");

        if (validTo <= DateTime.UtcNow)
            return new CtrlResponse<LoginResponse>("ValidTo is in the past");

        return user.TwoFactorEnabled
            ? new CtrlResponse<LoginResponse>(new LoginResponse(accessToken, user.RefreshToken, validTo,
                user.TwoFactorEnabled, user.Id))
            : new CtrlResponse<LoginResponse>(new LoginResponse(accessToken, user.RefreshToken, validTo,
                user.TwoFactorEnabled));
    }
}