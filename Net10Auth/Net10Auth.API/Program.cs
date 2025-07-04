using Net10Auth.API;
using Net10Auth.API.Middleware;
using Net10Auth.API.Services.Registration;
using Net10Auth.Shared.Infrastructure;
using Scalar.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json", false, true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", false, true);

builder.SetupDatabase();


builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration)
        .Enrich.FromLogContext());

builder.Services.Configure<JwtConfiguration>(builder.Configuration.GetSection("Jwt"));
builder.Services.AddSingleton<ITokenHandler, TokenHandler>();

builder.Services.AddControllers();

builder.Services.AddOpenApi();


builder.SetupIdentityCore();

builder.SetupEmailClient();

builder.AddCorsPolicy();

builder.RegisterServices();

builder.SetupJwtAuthentication();



var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseLoggingMiddleware();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseCors(x => x.AllowAnyOrigin()
    .AllowAnyHeader()
    .AllowAnyMethod()
);

app.Run();