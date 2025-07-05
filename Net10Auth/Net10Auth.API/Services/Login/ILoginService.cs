using Net10Auth.Shared.ControllerResponses;
using Net10Auth.Shared.Models.Identity;

namespace Net10Auth.API.Services.Login;

public interface ILoginService
{
    Task<CtrlResponse<LoginResponse>> LoginAsync(LoginInputModel model, HttpRequest httpRequest);
}