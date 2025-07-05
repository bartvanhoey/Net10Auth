using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Net10Auth.API.Controllers.Identity;
using Net10Auth.Shared.Infrastructure;
using Net10Auth.Shared.Infrastructure.Extensions;
using Net10Auth.Shared.Infrastructure.Functional;

namespace Net10Auth.API.Database;

public static class ApplicationUserExtensions
{
    public static ApplicationUser SetRefreshToken(this ApplicationUser user, IConfiguration configuration)
    {
        var refreshTokenExpiryInHours = 24;    
        if (int.TryParse(configuration["Jwt:RefreshTokenExpiryInHours"] ?? "24", out var expiry))
            refreshTokenExpiryInHours = expiry;
        
        var randomNumber = new byte[64];
        using var generator = RandomNumberGenerator.Create();
        generator.GetBytes(randomNumber);
            
        var refreshToken = Convert.ToBase64String(randomNumber);
            
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiry = DateTime.UtcNow.AddHours(refreshTokenExpiryInHours);            
        return user;
    }


    public static async Task<(string accessToken, DateTime validTo)> GenerateAccessToken(this ApplicationUser user,
        UserManager<ApplicationUser> userManager, JwtConfiguration jwt, HttpRequest httpRequest)
    {
        var authClaims = new List<Claim>
        {
            new(ClaimTypes.Name, user.Email ?? throw new InvalidOperationException()),
            new(ClaimTypes.NameIdentifier, user.Id),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var userRoles = await userManager.GetRolesAsync(user);
        if (userRoles is { Count: > 0 })
            authClaims.AddRange(userRoles.Select(userRole => new Claim(ClaimTypes.Role, userRole)));
        
        var userClaims = await userManager.GetClaimsAsync(user);
        authClaims.AddRange(userClaims);

        var origin = httpRequest.Headers.Origin.FirstOrDefault();
        if (origin.IsNullOrWhiteSpace() || !jwt.ValidAudiences.Contains(origin ?? throw new InvalidOperationException()))
            throw new InvalidOperationException("Invalid audience");
        
        var token = new JwtSecurityToken(
            jwt.ValidIssuer,
            origin,
            expires: DateTime.UtcNow.AddSeconds(double.Parse(jwt.AccessTokenExpiryInSeconds)), // 1 hour = 3600 sec
            claims: authClaims,
            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.SecurityKey)),
                SecurityAlgorithms.HmacSha256)
        );
        return (new JwtSecurityTokenHandler().WriteToken(token), token.ValidTo);






    }


    public static async Task AddToUserRoleAsync(this ApplicationUser user, UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        if (!await roleManager.RoleExistsAsync(UserRoles.User))
            await roleManager.CreateAsync(new IdentityRole(UserRoles.User));

        await userManager.AddToRoleAsync(user, UserRoles.User);
    }

    public static async Task AddToAdminRoleIfAdministratorAsync(this ApplicationUser user,
        UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
    {
        var admins = configuration.GetSection("ProgramAdministrators").Get<List<string>>();
        if (admins != null && admins.Contains(user.Email ?? throw new InvalidOperationException()))
        {
            if (!await roleManager.RoleExistsAsync(UserRoles.Admin))
                await roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));

            await userManager.AddToRoleAsync(user, UserRoles.Admin);
        }
    }
}