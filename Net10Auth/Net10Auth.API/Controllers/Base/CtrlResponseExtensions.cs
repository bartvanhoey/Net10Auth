using Net10Auth.Shared.ControllerResponses;

namespace Net10Auth.API.Controllers.Base;

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