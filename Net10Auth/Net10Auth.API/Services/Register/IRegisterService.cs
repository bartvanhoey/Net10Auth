using Net10Auth.API.Controllers;
using Net10Auth.API.Controllers.Identity;
using Net10Auth.Shared.ControllerResponses;
using Net10Auth.Shared.Models.Identity;


namespace Net10Auth.API.Services.Register;

public interface IRegisterService
{
    Task<CtrlResponse<RegisterResponse>> RegisterAsync(RegisterInputModel model, HttpRequest request);
}