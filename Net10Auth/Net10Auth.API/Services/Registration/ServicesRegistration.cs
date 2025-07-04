using Microsoft.Identity.Client;
using Net10Auth.API.Services.Register;
using Net10Auth.Shared.Infrastructure;

namespace Net10Auth.API.Services.Registration;

public static class ServicesRegistration
{
    public static void RegisterServices(this WebApplicationBuilder builder)
    {
        // builder.Services.AddScoped<IApiKeyApiService, ApiKeyApiService>();
        // builder.Services.AddScoped<IExceptionLogApiService, ExceptionLogApiService>();
        // builder.Services.AddScoped<ITwilioVerifyService, TwilioVerifyService>();
        // builder.Services.AddScoped<IUserApiService, UserApiService>();
        // builder.Services.AddScoped<Encryptor>();
         builder.Services.AddScoped<IRegisterService, RegisterService>();
    }
    
 
    
}